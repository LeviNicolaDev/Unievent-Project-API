using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IParticipacaoRepository
{
    Task<bool> TryConfirmarPresencaAsync(Participacao participacao);
    Task<Participacao> AddAsync(Participacao participacao);
    Task<Participacao> Update(Participacao participacao);
    Task SaveChangesAsync();
    Task<Participacao?> GetByAlunoIdAndEventoIdAsync(int alunoId, int eventoId);
    Task<bool> ExistsAsync(int alunoId, int eventoId);
    Task<int> CountByEventoIdAsync(int eventoId);
    Task<Participacao?> GetByCodigoIngressoAsync(string codigoIngresso);
    Task<IReadOnlyCollection<Participacao>> ListByAlunoIdAsync(int alunoId);
    Task<bool> TryAddWithinCapacityAsync(Participacao participacao, int capacidade);

}
