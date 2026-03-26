namespace Unievent.Application.Interfaces.Services;

public interface IUsuarioSecretariaService
{
    public Task<UsuarioSecretariaResponse> CriarUsuarioSecretaria(UsuarioSecretariaRequest request);
    public Task<UsuarioSecretariaResponse> AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update);
    public Task<bool> DeletarUsuarioSecretaria(int id);
    public Task<IList<UsuarioSecretariaResponse>> ListarUsuarioSecretaria();
    public Task<UsuarioSecretariaResponse> ListarUsuarioSecretariaById(int id);
}
