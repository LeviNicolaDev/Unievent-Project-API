using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Unievent.Application.Configurations.Email;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Services;
using Unievent.Application.Templates;
using Unievent.Infra.Services;
using Xunit;

namespace Unievent.Tests.Application.Email;

public class EmailAttachmentTest
{
    [Fact]
    public async Task SMTP_Recebe_Email_MIME_Com_Pdf_Real_Em_Anexo()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        var recebido = ReceberAsync(listener, timeout.Token);
        var arquivo = new CertificadoPdfGenerator().Gerar(new CertificadoPdfDados(
            "João Silva", "Semana de Tecnologia", "FATEC Ferraz", new DateTime(2026, 9, 10),
            "Carga horária: 4 horas.", "codigo-de-teste", DateTime.UtcNow, "Professora Érica"));
        var service = new EmailService(Options.Create(new EmailSettings
        {
            Host = "127.0.0.1", Port = port, Email = "unievent@example.com", EnableSsl = false
        }), NullLogger<EmailService>.Instance);
        var resultado = await service.SendWithAttachmentAsync("joao@example.com", "Seu certificado",
            "<p>PDF em anexo.</p>", new EmailAnexo(arquivo.Conteudo, arquivo.NomeArquivo),
            "<certificado-teste@unievent.local>", timeout.Token);
        resultado.Situacao.Should().Be(SituacaoEnvioEmail.Enviado);
        var mime = await recebido;
        mime.Should().Contain("To: joao@example.com").And.Contain("Content-Type: application/pdf")
            .And.Contain("Content-Disposition: attachment").And.Contain(arquivo.NomeArquivo);
        var partePdf = mime[mime.IndexOf("Content-Type: application/pdf", StringComparison.Ordinal)..];
        var anexado = Regex.Match(partePdf, @"Content-Transfer-Encoding: base64\r?\n(?:[^\r\n]+\r?\n)*\r?\n(?<body>[A-Za-z0-9+/=\r\n]+)", RegexOptions.IgnoreCase);
        anexado.Success.Should().BeTrue();
        Convert.FromBase64String(anexado.Groups["body"].Value).Should().Equal(arquivo.Conteudo);
    }

    private static async Task<string> ReceberAsync(TcpListener listener, CancellationToken ct)
    {
        using var client = await listener.AcceptTcpClientAsync(ct);
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.ASCII);
        using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };
        await writer.WriteLineAsync("220 localhost SMTP teste");
        var mime = new StringBuilder();
        while (await reader.ReadLineAsync(ct) is { } line)
        {
            if (line.StartsWith("DATA", StringComparison.OrdinalIgnoreCase))
            {
                await writer.WriteLineAsync("354 End with a dot");
                while (await reader.ReadLineAsync(ct) is { } data && data != ".")
                    mime.Append(data.StartsWith("..") ? data[1..] : data).Append("\r\n");
                await writer.WriteLineAsync("250 Accepted");
            }
            else if (line.StartsWith("QUIT", StringComparison.OrdinalIgnoreCase))
            {
                await writer.WriteLineAsync("221 Bye");
                break;
            }
            else await writer.WriteLineAsync("250 OK");
        }
        return mime.ToString();
    }

    [Fact]
    public void Falhas_SMTP_Distinguem_Rejeicao_De_Resultado_Incerto()
    {
        EmailService.ClassificarFalha(new SmtpException(SmtpStatusCode.ServiceNotAvailable), true)
            .Should().Be(SituacaoEnvioEmail.FalhaTemporaria);
        EmailService.ClassificarFalha(new SmtpException(SmtpStatusCode.MailboxUnavailable), true)
            .Should().Be(SituacaoEnvioEmail.FalhaPermanente);
        EmailService.ClassificarFalha(new TimeoutException(), true).Should().Be(SituacaoEnvioEmail.Incerto);
        EmailService.ClassificarFalha(new FormatException(), false).Should().Be(SituacaoEnvioEmail.FalhaPermanente);
    }

    [Fact]
    public void Template_Nao_Interpreta_HTML_Do_Participante_Ou_Evento()
    {
        var html = EmailTemplates.CertificadoDisponivel("<script>nome</script>", "<b>evento</b>", "<img>", "codigo");
        html.Should().NotContain("<script>").And.NotContain("<b>evento</b>").And.NotContain("<img>")
            .And.Contain("&lt;b&gt;evento&lt;/b&gt;");
    }
}
