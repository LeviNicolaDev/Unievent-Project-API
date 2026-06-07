using Microsoft.EntityFrameworkCore;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;

namespace Unievent.Infra.Repository;

public class ParticipacaoRepository : IParticipacaoRepository
{
    private readonly AppDbContext _context;
    public ParticipacaoRepository(AppDbContext context)
    {
        _context = context;
    }
    async Task<Participacao> IParticipacaoRepository.AddAsync(Participacao participacao)
    {
        var participacaoAdd = await _context.Participacao.AddAsync(participacao);
        return participacaoAdd.Entity;
    }

    async Task<bool> IParticipacaoRepository.ExistsAsync(int alunoId, int eventoId)
    {
        return await _context.Participacao.AnyAsync(p => p.AlunoId == alunoId && p.EventoId == eventoId);
    }

    async Task<Participacao?> IParticipacaoRepository.GetByAlunoIdAndEventoIdAsync(int alunoId, int eventoId)
    {
        return await _context.Participacao.FirstOrDefaultAsync(p => p.AlunoId == alunoId && p.EventoId == eventoId);
    }

    async Task IParticipacaoRepository.SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


    async Task<Participacao> IParticipacaoRepository.Update(Participacao participacao)
    {
        _context.Participacao.Update(participacao);
        return participacao;
    }
}
