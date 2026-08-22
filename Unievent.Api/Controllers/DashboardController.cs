using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Api.Security;
using Unievent.Application.Dtos.Dashboard;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet("admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminDashboardResponse>> AdminDashboard()
    {
        if (!User.IsGlobalAdmin()) return Forbid();

        var totalInscricoes = await db.Participacao.CountAsync();
        var totalPresencas = await db.Participacao.CountAsync(p => p.PresencaConfirmada);
        var instituicoes = await db.Instituicao
            .IgnoreQueryFilters()
            .AsNoTracking()
            .OrderBy(i => i.Nome)
            .Select(i => new
            {
                i.Id,
                Nome = i.Nome ?? i.NomeAbreviado ?? $"Instituição #{i.Id}",
                i.IsAtivo,
                Eventos = db.Evento.Count(e => e.InstituicaoId == i.Id),
                Inscricoes = db.Participacao.Count(p => p.Evento.InstituicaoId == i.Id),
                Presencas = db.Participacao.Count(p => p.Evento.InstituicaoId == i.Id && p.PresencaConfirmada),
                CertificadosEmitidos = db.Participacao.Count(p => p.Evento.InstituicaoId == i.Id && p.CertificadoEmitido)
            })
            .ToListAsync();

        return Ok(new AdminDashboardResponse
        {
            TotalInstituicoes = await db.Instituicao.IgnoreQueryFilters().CountAsync(),
            InstituicoesAtivas = await db.Instituicao.IgnoreQueryFilters().CountAsync(i => i.IsAtivo),
            TotalSecretarias = await db.UsuarioSecretaria.IgnoreQueryFilters().CountAsync(),
            SecretariasPendentes = await db.UsuarioSecretaria.IgnoreQueryFilters()
                .CountAsync(s => s.Status == StatusUsuarioSecretaria.Pendente),
            TotalAlunos = await db.Aluno.IgnoreQueryFilters().CountAsync(),
            TotalEventos = await db.Evento.CountAsync(),
            TotalInscricoes = totalInscricoes,
            TotalPresencas = totalPresencas,
            TotalCertificadosEmitidos = await db.Participacao.CountAsync(p => p.CertificadoEmitido),
            TaxaComparecimento = CalcularTaxa(totalPresencas, totalInscricoes),
            Instituicoes = instituicoes
                .Select(i => new AdminInstituicaoMetricasResponse
                {
                    InstituicaoId = i.Id,
                    InstituicaoNome = i.Nome,
                    Ativa = i.IsAtivo,
                    Eventos = i.Eventos,
                    Inscricoes = i.Inscricoes,
                    Presencas = i.Presencas,
                    CertificadosEmitidos = i.CertificadosEmitidos,
                    TaxaComparecimento = CalcularTaxa(i.Presencas, i.Inscricoes)
                })
                .ToList()
        });
    }

    [HttpGet("instituicao/dashboard")]
    [Authorize(Roles = "Secretaria")]
    public async Task<ActionResult<InstituicaoDashboardResponse>> InstituicaoDashboard()
    {
        var instituicaoId = User.GetInstituicaoId();
        if (!instituicaoId.HasValue) return Forbid();

        var instituicao = await db.Instituicao
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(i => i.Id == instituicaoId.Value)
            .Select(i => new
            {
                i.Id,
                Nome = i.Nome ?? i.NomeAbreviado ?? $"Instituição #{i.Id}"
            })
            .FirstOrDefaultAsync();

        if (instituicao is null) return Forbid();

        var agora = DateTime.UtcNow;
        var eventosQuery = db.Evento
            .AsNoTracking()
            .Where(e => e.InstituicaoId == instituicaoId.Value);
        var inscricoesQuery = db.Participacao
            .AsNoTracking()
            .Where(p => p.Evento.InstituicaoId == instituicaoId.Value);
        var totalInscricoes = await inscricoesQuery.CountAsync();
        var totalPresencas = await inscricoesQuery.CountAsync(p => p.PresencaConfirmada);

        var metricasPorEvento = await eventosQuery
            .OrderByDescending(e => e.DataEvento)
            .Select(e => new
            {
                e.Id,
                e.Nome,
                e.DataEvento,
                e.Capacidade,
                Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id),
                Presentes = db.Participacao.Count(p => p.EventoId == e.Id && p.PresencaConfirmada),
                CertificadosEmitidos = db.Participacao.Count(p => p.EventoId == e.Id && p.CertificadoEmitido)
            })
            .Take(100)
            .ToListAsync();

        var proximosEventos = await eventosQuery
            .Where(e => e.DataEvento >= agora)
            .OrderBy(e => e.DataEvento)
            .Select(e => new DashboardEventoResumoResponse
            {
                Id = e.Id,
                Nome = e.Nome,
                DataEvento = e.DataEvento,
                Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id),
                Presencas = db.Participacao.Count(p => p.EventoId == e.Id && p.PresencaConfirmada),
                CertificadosEmitidos = db.Participacao.Count(p => p.EventoId == e.Id && p.CertificadoEmitido)
            })
            .Take(5)
            .ToListAsync();

        var eventosRecentes = await eventosQuery
            .OrderByDescending(e => e.DataEvento)
            .Select(e => new DashboardEventoResumoResponse
            {
                Id = e.Id,
                Nome = e.Nome,
                DataEvento = e.DataEvento,
                Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id),
                Presencas = db.Participacao.Count(p => p.EventoId == e.Id && p.PresencaConfirmada),
                CertificadosEmitidos = db.Participacao.Count(p => p.EventoId == e.Id && p.CertificadoEmitido)
            })
            .Take(5)
            .ToListAsync();

        return Ok(new InstituicaoDashboardResponse
        {
            InstituicaoId = instituicao.Id,
            InstituicaoNome = instituicao.Nome,
            TotalEventos = await eventosQuery.CountAsync(),
            EventosFuturos = await eventosQuery.CountAsync(e => e.DataEvento >= agora),
            EventosRealizados = await eventosQuery.CountAsync(e => e.DataEvento < agora),
            TotalResponsaveis = await db.ResponsavelEvento.CountAsync(r => r.InstituicaoId == instituicaoId.Value),
            TotalInscricoes = totalInscricoes,
            TotalPresencas = totalPresencas,
            TotalCertificadosEmitidos = await inscricoesQuery.CountAsync(p => p.CertificadoEmitido),
            TaxaComparecimento = CalcularTaxa(totalPresencas, totalInscricoes),
            EventosRecentes = eventosRecentes,
            ProximosEventos = proximosEventos,
            MetricasPorEvento = metricasPorEvento
                .Select(e => new DashboardEventoMetricasResponse
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    DataEvento = e.DataEvento,
                    Capacidade = e.Capacidade,
                    Inscricoes = e.Inscricoes,
                    Presentes = e.Presentes,
                    Ausentes = e.Inscricoes - e.Presentes,
                    CertificadosEmitidos = e.CertificadosEmitidos,
                    TaxaComparecimento = CalcularTaxa(e.Presentes, e.Inscricoes)
                })
                .ToList()
        });
    }

    private static double CalcularTaxa(int parte, int total)
    {
        return total == 0 ? 0 : Math.Round(parte * 100d / total, 1);
    }
}
