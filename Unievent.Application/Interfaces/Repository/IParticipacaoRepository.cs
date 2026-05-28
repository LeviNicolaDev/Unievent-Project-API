using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IParticipacaoRepository
{
    Task<Participacao> AddAsync(Participacao participacao);
    Task<Participacao> Update(Participacao participacao);
    Task SaveChangesAsync();
    Task<Participacao?> GetByAlunoIdAndEventoIdAsync(int alunoId, int eventoId);
    Task<bool> ExistsAsync(int alunoId, int eventoId);

}
