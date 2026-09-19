using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Common;
using Unievent.Application.Configurations.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Templates;

namespace Unievent.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }


        public async Task<Result<bool>> SendAsync(string destinatario, string assunto, string mensagemHtml)
        {
            var result = await SendCoreAsync(destinatario, assunto, mensagemHtml, null, null, CancellationToken.None);
            return result.Situacao == SituacaoEnvioEmail.Enviado
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(result.Erro ?? "Erro ao enviar e-mail");
        }

        public Task<EnvioEmailResultado> SendWithAttachmentAsync(
            string destinatario, string assunto, string mensagemHtml, EmailAnexo anexo,
            string messageId, CancellationToken cancellationToken = default) =>
            SendCoreAsync(destinatario, assunto, mensagemHtml, anexo, messageId, cancellationToken);

        private async Task<EnvioEmailResultado> SendCoreAsync(
            string destinatario, string assunto, string mensagemHtml, EmailAnexo? anexo,
            string? messageId, CancellationToken cancellationToken)
        {
            var envioIniciado = false;
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Email),
                    Subject = assunto,
                    Body = mensagemHtml,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                message.To.Add(destinatario);
                if (messageId is not null) message.Headers.Add("Message-ID", messageId);
                if (anexo is not null)
                {
                    message.Attachments.Add(new Attachment(
                        new MemoryStream(anexo.Conteudo, writable: false), anexo.NomeArquivo, anexo.ContentType));
                }

                using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port);

                smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
                smtpClient.EnableSsl = _emailSettings.EnableSsl;
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeout.CancelAfter(TimeSpan.FromSeconds(30));
                cancellationToken.ThrowIfCancellationRequested();
                envioIniciado = true;
                await smtpClient.SendMailAsync(message, timeout.Token);

                return new(SituacaoEnvioEmail.Enviado);
            }
            catch (Exception ex)
            {
                var situacao = ClassificarFalha(ex, envioIniciado);
                _logger.LogWarning("Envio SMTP terminou com {Situacao}; tipo de erro {Tipo}", situacao, ex.GetType().Name);
                return new(situacao, situacao switch
                {
                    SituacaoEnvioEmail.FalhaTemporaria => "Servidor de e-mail indisponível; nova tentativa agendada.",
                    SituacaoEnvioEmail.FalhaPermanente => "Envio recusado ou configuração de e-mail inválida; requer correção.",
                    _ => "Resultado do envio incerto; verificar no provedor antes de reenviar."
                });
            }
        }

        public static SituacaoEnvioEmail ClassificarFalha(Exception error, bool envioIniciado)
        {
            if (!envioIniciado) return SituacaoEnvioEmail.FalhaPermanente;
            // Uma rejeição explícita do SMTP ou impossibilidade de conectar não representa entrega.
            if (error is SmtpException smtp && smtp.StatusCode != SmtpStatusCode.GeneralFailure)
                return (int)smtp.StatusCode is >= 400 and < 500
                    ? SituacaoEnvioEmail.FalhaTemporaria : SituacaoEnvioEmail.FalhaPermanente;
            for (Exception? inner = error; inner is not null; inner = inner.InnerException)
                if (inner is SocketException socket && socket.SocketErrorCode is
                    SocketError.ConnectionRefused or SocketError.HostNotFound or SocketError.NetworkUnreachable)
                    return SituacaoEnvioEmail.FalhaTemporaria;
            // Timeout, desconexão ou cancelamento após início podem ocorrer depois da aceitação.
            return SituacaoEnvioEmail.Incerto;
        }

        public async Task<Result<bool>> EnviarEmailConfirmacaoConta(string email, string nome, string chave)
        {
            _logger.LogInformation("Enviando e-mail de confirmação para {Email}", email);
            return await SendAsync(
                email,
                "Confirmação de Conta - Unievent",
                EmailTemplates.ConfirmacaoConta(nome, chave, _emailSettings.BaseUrl, _emailSettings.ConfirmationBaseUrl));
        }
    }
}
