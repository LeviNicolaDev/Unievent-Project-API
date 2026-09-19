using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Unievent.Application.Configurations;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Enuns;
using Unievent.Infra.Services;
using Unievent.Tests.Infrastructure;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using Xunit;

namespace Unievent.Tests.Infra;

public class CertificadoAutomaticoServiceTest
{
    private static CertificadoAutomaticoService Service(CertificacaoDatabase db, IEmailService email,
        ICertificadoPdfGenerator? pdf = null) => new(db, email, pdf ?? new CertificadoPdfGenerator(),
            Options.Create(new CertificacaoSettings()), NullLogger<CertificadoAutomaticoService>.Instance);

    private static Mock<IEmailService> Email(SituacaoEnvioEmail situacao = SituacaoEnvioEmail.Enviado)
    {
        var mock = new Mock<IEmailService>();
        mock.Setup(e => e.SendWithAttachmentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<EmailAnexo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EnvioEmailResultado(situacao, situacao == SituacaoEnvioEmail.Enviado ? null : "Falha SMTP de teste"));
        return mock;
    }

    [Theory]
    [InlineData(TipoParticipante.Interno, "joao@fatec.sp.gov.br")]
    [InlineData(TipoParticipante.Externo, "joao@example.com")]
    public async Task Gera_Pdf_Com_Dados_Reais_E_Envia_Anexo_Para_Ambos_Perfis(TipoParticipante tipo, string destinatario)
    {
        await using var db = new CertificacaoDatabase();
        var original = await db.SeedAsync(tipo: tipo);
        var email = Email();
        var result = await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        result.IsSuccess.Should().BeTrue();
        var p = await db.ParticipacaoAsync();
        p.CertificadoEmitido.Should().BeTrue();
        p.CertificadoEnviadoPorEmail.Should().BeTrue();
        p.StatusEnvioCertificado.Should().Be(StatusEnvioCertificado.Enviado);
        p.DataGeracaoCertificado.Should().NotBeNull();
        p.DataEnvioCertificadoEmail.Should().NotBeNull();
        p.NomeArquivoCertificado.Should().Be("certificado-semana-de-tecnologia-e-inovacao-joao-goncalves-da-silva.pdf");
        using var pdf = PdfDocument.Open(p.CertificadoPdf!);
        var text = string.Join(" ", pdf.GetPages().Select(page => ContentOrderTextExtractor.GetText(page)));
        text.Should().Contain(original.Aluno.Nome).And.Contain(original.Evento.Nome)
            .And.Contain("FATEC Ferraz de Vasconcelos").And.Contain("Carga horária: 4 horas")
            .And.Contain("Professora Érica").And.Contain(p.CodigoValidacao!)
            .And.Contain(original.Evento.DataEvento.ToString("dd/MM/yyyy"));
        pdf.NumberOfPages.Should().Be(1);
        email.Verify(e => e.SendWithAttachmentAsync(destinatario, It.Is<string>(s => s.Contains(original.Evento.Nome)),
            It.Is<string>(s => s.Contains("anexado")),
            It.Is<EmailAnexo>(a => a.ContentType == "application/pdf" && a.NomeArquivo == p.NomeArquivoCertificado && a.Conteudo.SequenceEqual(p.CertificadoPdf!)),
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Nao_Emite_Sem_Template_Ou_Sem_Presenca(bool certificado, bool presente)
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync(certificado, presente);
        var email = Email();
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        var p = await db.ParticipacaoAsync();
        p.CertificadoEmitido.Should().BeFalse();
        p.CertificadoPdf.Should().BeNull();
        email.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task Chamadas_Repetidas_Preservam_Pdf_Codigo_E_Um_Unico_Email()
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync();
        var email = Email();
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        var first = await db.ParticipacaoAsync();
        for (var i = 0; i < 3; i++) await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        var last = await db.ParticipacaoAsync();
        last.CertificadoPdf.Should().Equal(first.CertificadoPdf!);
        last.CodigoValidacao.Should().Be(first.CodigoValidacao);
        last.DataEnvioCertificadoEmail.Should().Be(first.DataEnvioCertificadoEmail);
        email.Invocations.Should().HaveCount(1);
    }

    [Fact]
    public async Task Reservas_Entre_Instancias_Impedem_Envio_Concorrente()
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync();
        var entrou = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var concluir = new TaskCompletionSource<EnvioEmailResultado>(TaskCreationOptions.RunContinuationsAsynchronously);
        var email = Email();
        email.Setup(e => e.SendWithAttachmentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<EmailAnexo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(() => { entrou.TrySetResult(); return concluir.Task; });
        var primeira = Service(db, email.Object).ProcessarAposCheckInAsync(1);
        await entrou.Task.WaitAsync(TimeSpan.FromSeconds(15));
        try
        {
            var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Service(db, email.Object).ProcessarAposCheckInAsync(1)));
            results.Should().OnlyContain(r => r.IsSuccess);
            email.Invocations.Should().HaveCount(1);
        }
        finally { concluir.TrySetResult(new(SituacaoEnvioEmail.Enviado)); }
        await primeira;
        (await db.ParticipacaoAsync()).CertificadoEnviadoPorEmail.Should().BeTrue();
    }

    [Fact]
    public async Task Falha_Temporaria_Preserva_CheckIn_Pdf_E_Retenta_Somente_Quando_Devido()
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync();
        var email = Email(SituacaoEnvioEmail.FalhaTemporaria);
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        var first = await db.ParticipacaoAsync();
        first.PresencaConfirmada.Should().BeTrue();
        first.CertificadoEmitido.Should().BeTrue();
        first.CertificadoPdf.Should().NotBeNull();
        first.StatusEnvioCertificado.Should().Be(StatusEnvioCertificado.FalhaTemporaria);
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        email.Invocations.Should().HaveCount(1);
        await using (var context = db.CreateDbContext())
            await context.Participacao.ExecuteUpdateAsync(s => s.SetProperty(p => p.ProximaTentativaCertificadoUtc, DateTime.UtcNow.AddMinutes(-1)));
        var sucesso = Email();
        await Service(db, sucesso.Object).ProcessarAposCheckInAsync(1);
        var final = await db.ParticipacaoAsync();
        final.CertificadoEnviadoPorEmail.Should().BeTrue();
        final.CertificadoPdf.Should().Equal(first.CertificadoPdf!);
        final.CodigoValidacao.Should().Be(first.CodigoValidacao);
    }

    [Theory]
    [InlineData(SituacaoEnvioEmail.Incerto, StatusEnvioCertificado.EnvioIncerto)]
    [InlineData(SituacaoEnvioEmail.FalhaPermanente, StatusEnvioCertificado.FalhaPermanente)]
    public async Task Nao_Retenta_Resultado_Incerto_Ou_Falha_Permanente(SituacaoEnvioEmail resultado, StatusEnvioCertificado esperado)
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync();
        var email = Email(resultado);
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        (await db.ParticipacaoAsync()).StatusEnvioCertificado.Should().Be(esperado);
        email.Invocations.Should().HaveCount(1);
    }

    [Fact]
    public async Task Crash_Durante_SMTP_Nao_Libera_Reenvio_Automatico()
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync();
        await using (var context = db.CreateDbContext())
            await context.Participacao.ExecuteUpdateAsync(s => s
                .SetProperty(p => p.StatusEnvioCertificado, StatusEnvioCertificado.Enviando)
                .SetProperty(p => p.ProcessamentoCertificadoAteUtc, DateTime.UtcNow.AddMinutes(-1)));
        var email = Email();
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        (await db.ParticipacaoAsync()).StatusEnvioCertificado.Should().Be(StatusEnvioCertificado.EnvioIncerto);
        email.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task Pdf_Legado_Mantem_Codigo_E_Nao_Reenvia_Email_Ja_Registrado()
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync();
        await using (var context = db.CreateDbContext())
        {
            var p = await context.Participacao.SingleAsync();
            p.EmitirCertificado("codigo-legado");
            p.RegistrarEnvioCertificadoEmail();
            await context.SaveChangesAsync();
        }
        var email = Email();
        await Service(db, email.Object).ProcessarAposCheckInAsync(1);
        var saved = await db.ParticipacaoAsync();
        saved.CertificadoPdf.Should().NotBeNull();
        saved.CodigoValidacao.Should().Be("codigo-legado");
        email.Invocations.Should().BeEmpty();
    }

    [Fact]
    public async Task Erro_Ao_Gerar_Pdf_Preserva_Presenca_E_Nao_Marca_Emissao()
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync();
        var pdf = new Mock<ICertificadoPdfGenerator>();
        pdf.Setup(p => p.Gerar(It.IsAny<CertificadoPdfDados>())).Throws(new IOException("Falha de teste"));
        var email = Email();
        await Service(db, email.Object, pdf.Object).ProcessarAposCheckInAsync(1);
        var saved = await db.ParticipacaoAsync();
        saved.PresencaConfirmada.Should().BeTrue();
        saved.CertificadoEmitido.Should().BeFalse();
        saved.StatusEnvioCertificado.Should().Be(StatusEnvioCertificado.FalhaTemporaria);
        email.Invocations.Should().BeEmpty();
    }
}
