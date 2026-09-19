using Unievent.Application.Common;

namespace Unievent.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<Dtos.Email.EnvioEmailResultado> SendWithAttachmentAsync(
            string destinatario, string assunto, string mensagemHtml,
            Dtos.Email.EmailAnexo anexo, string messageId,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> SendAsync(
            string destinatario,
            string assunto,
            string mensagemHtml);

        Task<Result<bool>> EnviarEmailConfirmacaoConta(
            string email,
            string nome,
            string chave);
    }
}
