using Unievent.Application.Common;
using Unievent.Application.Dtos.UsuarioSecretaria;

namespace Unievent.Application.Interfaces.Services;

public interface IUsuarioSecretariaService
{
    public Task<Result<UsuarioSecretariaResponse>> CriarUsuarioSecretaria(UsuarioSecretariaRequest request);
    public Task<Result<UsuarioSecretariaResponse>> AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update);
    public Task<Result<bool>> DeletarUsuarioSecretaria(int id);
    public Task<Result<IEnumerable<UsuarioSecretariaResponse>>> ListarUsuarioSecretaria();
    public Task<Result<UsuarioSecretariaResponse>> ListarUsuarioSecretariaById(int id);
    public Task<Result<UsuarioSecretariaLoginResponse>> Login(UsuarioSecretariaLoginRequest request);
}
