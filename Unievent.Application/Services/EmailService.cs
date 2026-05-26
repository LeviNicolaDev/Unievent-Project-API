using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using Unievent.Application.Common;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _emailPassword;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;
            _emailPassword = configuration["emailPassword"] ?? configuration["PASSWORD_EMAIL"];
        }

        public async Task<Result<bool>> EnviarEmailConfirmacaoConta(string email, string nome, string chave)
        {
            try
            {
                _logger.LogInformation("Enviando email de confirmação de conta para {Email}", email);

                var enviarEmail = new EmailModel()
                {
                    Titulo = "Confirmação de Conta - Unievent",
                    Mensagem = TemplateEmailConfirmarConta(nome, chave),
                    Destinatario = email
                };

                var message = new MailMessage()
                {
                    From = new MailAddress("adm.unievent@gmail.com"),
                    Subject = enviarEmail.Titulo,
                    Body = enviarEmail.Mensagem
                };
                message.To.Add(email);

                using SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new System.Net.NetworkCredential("adm.unievent@gmail.com", _emailPassword);
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);

                _logger.LogInformation("Email de confirmação enviado com sucesso para {Email}", email);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de confirmação para {Email}", email);
                return Result<bool>.Failure("Erro ao enviar email de confirmação de conta");
            }
        }

        private string TemplateEmailConfirmarConta(string nome, string chave)
        {
            return $@"<html>
                        <head>
                            <meta charset=""UTF-8"">
                            <title>Confirmação de Conta</title>
                        </head>
                        <body>
                            <h1>Olá, {nome}!</h1>
                            <p>Obrigado por se registrar em nosso serviço. Para confirmar sua conta, por favor clique no link abaixo:</p>
                            <a href=""http://localhost/unievent-project/src/Service/ConfirmarEmail.php?chave={chave}"">Confirmar Conta</a>
                            <p>Se você não se registrou, por favor ignore este email.</p>
                            <p>Atenção: nunca solicitamos o envio de senhas, dados pessoais ou arquivos anexados por e-mail.<br><br>Em caso de dúvidas, entre em contato com nosso suporte.</p>
                            <p>Atenciosamente,<br>Equipe Unievent</p>
                        </body>
                    </html>";
        }
    }
}
