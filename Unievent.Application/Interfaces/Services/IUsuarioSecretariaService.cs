using Unievent.Application.Dtos.UsuarioSecretaria;

namespace Unievent.Application.Interfaces.Services;

public interface IUsuarioSecretariaService
{
    public Task<UsuarioSecretariaResponse> CriarUsuarioSecretaria(UsuarioSecretariaRequest request);
    public Task<UsuarioSecretariaResponse> AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update);
    public Task<bool> DeletarUsuarioSecretaria(int id);
    public Task<IEnumerable<UsuarioSecretariaResponse>> ListarUsuarioSecretaria();
    public Task<UsuarioSecretariaResponse> ListarUsuarioSecretariaById(int id);
    public Task<UsuarioSecretariaLoginResponse> Login(UsuarioSecretariaLoginRequest request);
}
