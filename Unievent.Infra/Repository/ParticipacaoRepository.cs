using Microsoft.EntityFrameworkCore;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;
using System.Data;

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
        return await _context.Participacao.Include(p => p.Aluno).Include(p => p.Evento).FirstOrDefaultAsync(p => p.AlunoId == alunoId && p.EventoId == eventoId);
    }

    Task<int> IParticipacaoRepository.CountByEventoIdAsync(int eventoId) =>
        _context.Participacao.CountAsync(p => p.EventoId == eventoId);

    Task<Participacao?> IParticipacaoRepository.GetByCodigoIngressoAsync(string codigoIngresso) =>
        _context.Participacao.Include(p => p.Aluno).Include(p => p.Evento)
            .FirstOrDefaultAsync(p => p.CodigoIngresso == codigoIngresso);

    async Task<bool> IParticipacaoRepository.TryAddWithinCapacityAsync(Participacao participacao, int capacidade)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var jaExiste = await _context.Participacao.AnyAsync(p =>
            p.AlunoId == participacao.AlunoId && p.EventoId == participacao.EventoId);
        var inscritos = await _context.Participacao.CountAsync(p => p.EventoId == participacao.EventoId);
        if (jaExiste || inscritos >= capacidade)
        {
            await transaction.RollbackAsync();
            return false;
        }

        await _context.Participacao.AddAsync(participacao);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
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
