using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IResponsavelEventoRepository
{
    public Task<ResponsavelEvento> CriarResponsavelEvento(ResponsavelEvento responsavel);
    public Task<ResponsavelEvento> AtualizarResponsavelEvento(ResponsavelEvento responsavel);
    public Task<IList<ResponsavelEvento>> ListarResponsaveisEvento();
    public Task<ResponsavelEvento> ListarResponsavelEventoById(int id);
    public Task SaveChangesAsync();
    public Task<bool> DeletarResponsavelEvento(ResponsavelEvento responsavelEvento);
}
