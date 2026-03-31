using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IUsuarioSecretariaRepository
{
    public Task<UsuarioSecretaria> CriarUsuarioSecretaria(UsuarioSecretaria usuarioSecretaria);
    public Task<UsuarioSecretaria> AtualizarUsuarioSecretaria(UsuarioSecretaria usuarioSecretaria);
    public Task<UsuarioSecretaria> ListarUsuarioSecretariaById(int id);
    public Task<UsuarioSecretaria> ListarUsuarioSecretariaByEmail(string email);
    public Task<IEnumerable<UsuarioSecretaria>> ListarUsuarioSecretarias();
    public Task<bool> DeletarUsuarioSecretaria(UsuarioSecretaria usuarioSecretaria);
    public Task SaveChangesAsync();
}
