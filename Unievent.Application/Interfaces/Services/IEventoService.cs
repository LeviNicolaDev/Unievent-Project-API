using Unievent.Application.Dtos.Evento;

namespace Unievent.Application.Interfaces.Services;

public interface IEventoService
{
    public Task<EventoResponse> CriarEvento(EventoRequest request);
    public Task<EventoResponse> AtualizarEvento(int id, EventoUpdate update);
    public Task<bool> DeletarEvento(int id);
    public Task<IEnumerable<EventoResponse>> ListarEventos();
    public Task<EventoResponse> ListarEventoById(int id);
}
