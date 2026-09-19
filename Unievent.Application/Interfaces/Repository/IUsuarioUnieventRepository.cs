using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IUsuarioUnieventRepository
{
    Task<UsuarioUnievent> CriarUsuarioUnievent(UsuarioUnievent usuarioUnievent);
    Task<UsuarioUnievent?> ListarUsuarioUnieventByEmail(string email);
    Task<bool> ExisteUsuarioUnievent();
    Task SaveChangesAsync();
}
