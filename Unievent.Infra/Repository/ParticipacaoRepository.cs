using Microsoft.EntityFrameworkCore;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;
using System.Data;
using Unievent.Domain.Enuns;

namespace Unievent.Infra.Repository;

public class ParticipacaoRepository : IParticipacaoRepository
{
    private readonly AppDbContext _context;
    public ParticipacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> TryConfirmarPresencaAsync(Participacao participacao)
    {
        // Somente a primeira validação grava operador/data; nenhum campo de certificado é sobrescrito.
        var updated = await _context.Participacao.Where(p => p.Id == participacao.Id &&
                !p.PresencaConfirmada && p.StatusInscricao == StatusInscricao.Ativa)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.PresencaConfirmada, true)
                .SetProperty(p => p.DataConfirmacao, participacao.DataConfirmacao)
                .SetProperty(p => p.OperadorCheckInId, participacao.OperadorCheckInId)
                .SetProperty(p => p.DistanciaCheckInMetros, participacao.DistanciaCheckInMetros)
                .SetProperty(p => p.PrecisaoLocalizacaoMetros, participacao.PrecisaoLocalizacaoMetros));
        return updated == 1;
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
        return await _context.Participacao.AsNoTracking()
            .Include(p => p.Aluno)
            .Include(p => p.Evento).ThenInclude(e => e.Instituicao)
            .FirstOrDefaultAsync(p => p.AlunoId == alunoId && p.EventoId == eventoId);
    }

    Task<int> IParticipacaoRepository.CountByEventoIdAsync(int eventoId) =>
        _context.Participacao.CountAsync(p => p.EventoId == eventoId && p.StatusInscricao == StatusInscricao.Ativa);

    Task<Participacao?> IParticipacaoRepository.GetByCodigoIngressoAsync(string codigoIngresso) =>
        _context.Participacao.AsNoTracking().Include(p => p.Aluno)
            .Include(p => p.Evento).ThenInclude(e => e.Instituicao)
            .FirstOrDefaultAsync(p => p.CodigoIngresso == codigoIngresso);

    async Task<IReadOnlyCollection<Participacao>> IParticipacaoRepository.ListByAlunoIdAsync(int alunoId) =>
        await _context.Participacao
            .AsNoTracking()
            .Include(p => p.Evento)
            .ThenInclude(e => e.Instituicao)
            .Where(p => p.AlunoId == alunoId)
            .OrderByDescending(p => p.Evento.DataEvento)
            .ToListAsync();

    async Task<bool> IParticipacaoRepository.TryAddWithinCapacityAsync(Participacao participacao, int capacidade)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var jaExiste = await _context.Participacao.AnyAsync(p =>
                p.AlunoId == participacao.AlunoId && p.EventoId == participacao.EventoId);
            var inscritos = await _context.Participacao.CountAsync(p =>
                p.EventoId == participacao.EventoId && p.StatusInscricao == StatusInscricao.Ativa);
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
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return false;
        }
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
