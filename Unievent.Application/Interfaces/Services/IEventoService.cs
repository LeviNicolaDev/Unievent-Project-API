using Unievent.Application.Common;
using Unievent.Application.Dtos.Evento;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Interfaces.Services;

public interface IEventoService
{
    public Task<Result<EventoResponse>> CriarEvento(EventoRequest request);
    public Task<Result<EventoResponse>> AtualizarEvento(int id, EventoUpdate update);
    public Task<Result<bool>> DeletarEvento(int id);
    public Task<Result<IEnumerable<EventoResponse>>> ListarEventos();
    public Task<Result<IList<EventoResponse>>> ListarEventosByCategoria(Categoria categoria);
    public Task<Result<EventoResponse>> ListarEventosByResponsavel(int responsavelId);
    public Task<Result<EventoResponse>> ListarEventoById(int id);
    public Task<Result<IEnumerable<EventoResponse>>> ListarEventosDisponiveis(TipoParticipante? tipoParticipante);
}
