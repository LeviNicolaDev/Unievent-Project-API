using Unievent.Application.Common;
using Unievent.Application.Dtos.Participacao;
using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Services;

public interface IParticipacaoService
{
    Task<Result<ParticipacaoResponse>> InscreverAsync(int alunoId, int eventoId);
    Task<Result<ParticipacaoResponse>> GarantirPresencaAsync(int alunoId, int eventoId);
    Task<Result<ParticipacaoResponse>> EmitirCertificadoAsync(int alunoId, int eventoId);
}
