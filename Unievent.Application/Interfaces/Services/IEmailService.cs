using Unievent.Application.Common;

namespace Unievent.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<Result<bool>> EnviarEmailConfirmacaoConta(string email, string nome, string chave);
    }
}
