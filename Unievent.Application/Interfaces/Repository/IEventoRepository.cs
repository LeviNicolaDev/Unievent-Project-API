using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Interfaces.Repository;

public interface IEventoRepository
{
    public Task<Evento> CriarEvento(Evento evento);
    public Task<Evento> AtualizarEvento(Evento evento);
    public Task<IEnumerable<Evento>> ListarEventos();
    public Task<Evento> ListarEventoById(int id);
    public Task<bool> DeletarEvento(Evento evento);
    public Task<IList<Evento>> ListarEventoByCategoria(Categoria categoria);
    public Task<Evento> ListarEventoByResponsavel(int responsavelId);
    public Task SaveChangesAsync();
}
