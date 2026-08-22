using Unievent.Application.Common;
using Unievent.Application.Dtos.Automacoes;

namespace Unievent.Application.Interfaces.Services;

public interface IAutomacaoEventosService
{
    Task<Result<AutomacaoEventosResult>> ProcessarAsync(CancellationToken cancellationToken = default);
}
