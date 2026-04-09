using Unievent.Application.Common;
using Unievent.Application.Dtos.Evento;

namespace Unievent.Application.Interfaces.Services;

public interface IEventoService
{
    public Task<ResultData<EventoResponse>> CriarEvento(EventoRequest request);
    public Task<ResultData<EventoResponse>> AtualizarEvento(int id, EventoUpdate update);
    public Task<Result> DeletarEvento(int id);
    public Task<ResultData<IEnumerable<EventoResponse>>> ListarEventos();
    public Task<ResultData<IEnumerable<EventoResponse>>> ListarEventosByCategoria(string categoria);
    public Task<ResultData<EventoResponse>> ListarEventosByResponsavel(int responsavelId);
    public Task<ResultData<EventoResponse>> ListarEventoById(int id);
}
