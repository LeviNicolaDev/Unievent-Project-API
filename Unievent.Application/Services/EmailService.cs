using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
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
            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.Email),
                    Subject = assunto,
                    Body = mensagemHtml,
                    IsBodyHtml = true
                };

                message.To.Add(destinatario);

                using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port);

                smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
                smtpClient.EnableSsl = _emailSettings.EnableSsl;
                await smtpClient.SendMailAsync(message);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail para {Email}", destinatario);
                return Result<bool>.Failure("Erro ao enviar e-mail");
            }
        }

        public async Task<Result<bool>> EnviarEmailConfirmacaoConta(string email, string nome, string chave)
        {
            _logger.LogInformation("Enviando e-mail de confirmação para {Email}", email);
            return await SendAsync(email, "Confirmação de Conta - Unievent", EmailTemplates.ConfirmacaoConta(nome, chave, _emailSettings.BaseUrl));
        }
    }
}