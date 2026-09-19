using Unievent.Application.Common;
using Unievent.Application.Dtos.Automacoes;

namespace Unievent.Application.Interfaces.Services;

public interface ICertificadoAutomaticoService
{
    Task<Result<CertificadoAutomaticoResult>> ProcessarAposCheckInAsync(
        int participacaoId,
        CancellationToken cancellationToken = default);
}
