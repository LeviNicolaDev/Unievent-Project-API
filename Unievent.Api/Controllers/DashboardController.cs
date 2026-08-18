using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin,Secretaria")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet("resumo")]
    public async Task<ActionResult> Resumo([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
    {
        var de = inicio ?? DateTime.UtcNow.Date.AddMonths(-1);
        var ate = fim ?? DateTime.UtcNow.Date.AddMonths(1);
        if (ate < de) return BadRequest("Período inválido");

        var eventos = db.Evento.AsNoTracking().Where(e => e.DataEvento >= de && e.DataEvento <= ate);
        var ids = eventos.Select(e => e.Id);
        var inscricoes = db.Participacao.AsNoTracking().Where(p => ids.Contains(p.EventoId));
        var totalInscricoes = await inscricoes.CountAsync();
        var presentes = await inscricoes.CountAsync(p => p.PresencaConfirmada);
        var internos = await inscricoes.CountAsync(p => p.Aluno.TipoParticipante == TipoParticipante.Interno);

        return Ok(new
        {
            inicio = de,
            fim = ate,
            eventos = await eventos.CountAsync(),
            inscricoes = totalInscricoes,
            presentes,
            taxaComparecimento = totalInscricoes == 0 ? 0 : Math.Round(presentes * 100d / totalInscricoes, 1),
            participantesInternos = internos,
            participantesExternos = totalInscricoes - internos,
            capacidadeTotal = await eventos.SumAsync(e => (int?)e.Capacidade) ?? 0
        });
    }

    [HttpGet("eventos")]
    public async Task<ActionResult> PorEvento() => Ok(await db.Evento.AsNoTracking()
        .OrderByDescending(e => e.DataEvento)
        .Select(e => new
        {
            e.Id, e.Nome, e.DataEvento, e.Capacidade,
            Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id),
            Presentes = db.Participacao.Count(p => p.EventoId == e.Id && p.PresencaConfirmada)
        }).Take(100).ToListAsync());
}
