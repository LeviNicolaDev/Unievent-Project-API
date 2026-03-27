using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IEventoRepository
{
    public Task<Evento> CriarEvento(Evento evento);
    public Task<Evento> AtualizarEvento(Evento evento);
    public Task<IEnumerable<Evento>> ListarEventos();
    public Task<Evento> ListarEventoById(int id);
    public Task<bool> DeletarEvento(Evento evento);
    public Task SaveChangesAsync();
}
