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

        var totalInscricoes = await db.Participacao.CountAsync(p => p.StatusInscricao == StatusInscricao.Ativa);
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
                Inscricoes = db.Participacao.Count(p => p.Evento.InstituicaoId == i.Id && p.StatusInscricao == StatusInscricao.Ativa),
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
            .Where(p => p.Evento.InstituicaoId == instituicaoId.Value && p.StatusInscricao == StatusInscricao.Ativa);
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
                Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id && p.StatusInscricao == StatusInscricao.Ativa),
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
                Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id && p.StatusInscricao == StatusInscricao.Ativa),
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
                Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id && p.StatusInscricao == StatusInscricao.Ativa),
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

    [HttpGet("Evento/{eventId:int}/dashboard")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<EventoDashboardResponse>> EventoDashboard([FromRoute] int eventId)
    {
        var evento = await db.Evento
            .AsNoTracking()
            .Include(e => e.Instituicao)
            .Include(e => e.ResponsavelEvento)
            .Where(e => e.Id == eventId)
            .Select(e => new
            {
                e.Id,
                e.Nome,
                e.DataEvento,
                e.Local,
                e.InstituicaoId,
                InstituicaoNome = e.Instituicao != null
                    ? e.Instituicao.Nome ?? e.Instituicao.NomeAbreviado ?? $"Instituição #{e.Instituicao.Id}"
                    : "Instituição não informada",
                e.ResponsavelEventoId,
                ResponsavelEventoNome = e.ResponsavelEvento != null ? e.ResponsavelEvento.Nome : string.Empty,
                e.PublicoPermitido,
                e.Capacidade,
                e.InicioInscricoes,
                e.FimInscricoes,
                Inscricoes = db.Participacao.Count(p => p.EventoId == e.Id && p.StatusInscricao == StatusInscricao.Ativa),
                CheckIns = db.Participacao.Count(p => p.EventoId == e.Id && p.PresencaConfirmada),
                CertificadosEmitidos = db.Participacao.Count(p => p.EventoId == e.Id && p.CertificadoEmitido),
                InscricoesPublicoGeral = db.Participacao.Count(p =>
                    p.EventoId == e.Id && p.StatusInscricao == StatusInscricao.Ativa &&
                    p.Aluno.TipoParticipante == TipoParticipante.Externo),
                InscricoesAlunosFatec = db.Participacao.Count(p =>
                    p.EventoId == e.Id && p.StatusInscricao == StatusInscricao.Ativa &&
                    p.Aluno.TipoParticipante == TipoParticipante.Interno),
                PossuiCertificado = db.Certificado.Any(c => c.EventoId == e.Id)
            })
            .FirstOrDefaultAsync();

        if (evento is null) return NotFound("Evento não encontrado");
        if (!User.CanAccessInstituicao(evento.InstituicaoId)) return Forbid();

        var vagasRestantes = Math.Max(evento.Capacidade - evento.Inscricoes, 0);

        return Ok(new EventoDashboardResponse
        {
            EventoId = evento.Id,
            Nome = evento.Nome,
            DataEvento = evento.DataEvento,
            Local = evento.Local,
            InstituicaoId = evento.InstituicaoId,
            InstituicaoNome = evento.InstituicaoNome,
            ResponsavelEventoId = evento.ResponsavelEventoId,
            ResponsavelEventoNome = evento.ResponsavelEventoNome,
            PublicoPermitido = evento.PublicoPermitido,
            CapacidadeTotal = evento.Capacidade,
            TotalInscricoes = evento.Inscricoes,
            VagasRestantes = vagasRestantes,
            CheckInsRealizados = evento.CheckIns,
            AusentesSemCheckIn = evento.Inscricoes - evento.CheckIns,
            PercentualOcupacao = CalcularTaxa(evento.Inscricoes, evento.Capacidade),
            PercentualPresenca = CalcularTaxa(evento.CheckIns, evento.Inscricoes),
            StatusEvento = ObterStatusEvento(evento.DataEvento, evento.InicioInscricoes, evento.FimInscricoes, vagasRestantes),
            PossuiCertificado = evento.PossuiCertificado,
            CertificadosEmitidos = evento.CertificadosEmitidos,
            InscricoesPublicoGeral = evento.InscricoesPublicoGeral,
            InscricoesAlunosFatec = evento.InscricoesAlunosFatec
        });
    }

    private static double CalcularTaxa(int parte, int total)
    {
        return total == 0 ? 0 : Math.Round(parte * 100d / total, 1);
    }

    private static string ObterStatusEvento(DateTime dataEvento, DateTime? inicioInscricoes, DateTime? fimInscricoes, int vagasRestantes)
    {
        var agora = DateTime.UtcNow;
        if (dataEvento.ToUniversalTime() < agora) return "Realizado";
        if (vagasRestantes <= 0) return "Lotado";
        if (inicioInscricoes.HasValue && agora < inicioInscricoes.Value.ToUniversalTime()) return "Inscrições não iniciadas";
        if (fimInscricoes.HasValue && agora > fimInscricoes.Value.ToUniversalTime()) return "Inscrições encerradas";
        return "Inscrições abertas";
    }
}
