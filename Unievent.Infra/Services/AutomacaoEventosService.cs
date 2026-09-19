using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Automacoes;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Rules;
using Unievent.Application.Templates;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Infra.Services;

public class AutomacaoEventosService : IAutomacaoEventosService
{
    private const string TipoAlertaEvento = "AlertaEvento";
    private readonly AppDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ICertificadoAutomaticoService _certificadoAutomaticoService;
    private readonly ILogger<AutomacaoEventosService> _logger;

    public AutomacaoEventosService(
        AppDbContext db,
        IEmailService emailService,
        ICertificadoAutomaticoService certificadoAutomaticoService,
        ILogger<AutomacaoEventosService> logger)
    {
        _db = db;
        _emailService = emailService;
        _certificadoAutomaticoService = certificadoAutomaticoService;
        _logger = logger;
    }

    public async Task<Result<AutomacaoEventosResult>> ProcessarAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var certificados = await ProcessarCertificadosAsync(cancellationToken);
            var alertas = await ProcessarAlertasAsync(cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return Result<AutomacaoEventosResult>.Success(new AutomacaoEventosResult(
                certificados.processados,
                alertas.processados,
                certificados.falhas + alertas.falhas));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar automações de eventos");
            return Result<AutomacaoEventosResult>.Failure("Erro ao processar automações de eventos");
        }
    }

    private async Task<(int processados, int falhas)> ProcessarCertificadosAsync(CancellationToken cancellationToken)
    {
        var agora = DateTime.UtcNow;
        var participacaoIds = await _db.Participacao
            .AsNoTracking()
            .Where(p => p.PresencaConfirmada &&
                        (p.CertificadoPdf != null || _db.Certificado.Any(c => c.EventoId == p.EventoId)) &&
                        (!p.CertificadoEnviadoPorEmail || p.CertificadoPdf == null) &&
                        (p.ProximaTentativaCertificadoUtc == null || p.ProximaTentativaCertificadoUtc <= agora) &&
                        (p.StatusEnvioCertificado == StatusEnvioCertificado.Pendente ||
                         p.StatusEnvioCertificado == StatusEnvioCertificado.FalhaTemporaria ||
                         p.StatusEnvioCertificado == StatusEnvioCertificado.Enviado && p.CertificadoPdf == null ||
                         (p.StatusEnvioCertificado == StatusEnvioCertificado.Preparando ||
                          p.StatusEnvioCertificado == StatusEnvioCertificado.Enviando) && p.ProcessamentoCertificadoAteUtc <= agora))
            .OrderBy(p => p.ProximaTentativaCertificadoUtc).ThenBy(p => p.DataConfirmacao)
            .Select(p => p.Id)
            .Take(200)
            .ToListAsync(cancellationToken);

        if (participacaoIds.Count == 0) return (0, 0);

        var processados = 0;
        var falhas = 0;

        foreach (var participacaoId in participacaoIds)
        {
            var result = await _certificadoAutomaticoService.ProcessarAposCheckInAsync(participacaoId, cancellationToken);
            if (result.IsFailure) falhas++;
            else if (result.Value?.ErroEmail is not null) falhas++;
            else if (result.Value?.CertificadoConfigurado == true) processados++;
        }

        return (processados, falhas);
    }

    private async Task<(int processados, int falhas)> ProcessarAlertasAsync(CancellationToken cancellationToken)
    {
        var agora = DateTime.UtcNow;
        var limite = agora.AddDays(14);
        var eventos = await _db.Evento
            .AsNoTracking()
            .Where(e => e.DataEvento > agora && e.DataEvento <= limite)
            .OrderBy(e => e.DataEvento)
            .Take(100)
            .ToListAsync(cancellationToken);

        if (eventos.Count == 0) return (0, 0);

        var alunos = await _db.Aluno
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var preferencias = await _db.PreferenciaNotificacao
            .AsNoTracking()
            .ToDictionaryAsync(p => p.AlunoId, cancellationToken);
        var eventoIds = eventos.Select(e => e.Id).ToArray();
        var notificacoesExistentes = await _db.NotificacaoEvento
            .AsNoTracking()
            .Where(n => n.Tipo == TipoAlertaEvento && eventoIds.Contains(n.EventoId))
            .Select(n => n.AlunoId + ":" + n.EventoId)
            .ToListAsync(cancellationToken);
        var jaProcessadas = notificacoesExistentes.ToHashSet(StringComparer.Ordinal);

        var processados = 0;
        var falhas = 0;

        foreach (var aluno in alunos)
        {
            if (!preferencias.TryGetValue(aluno.Id, out var preferencia) ||
                !preferencia.LembretesEventos ||
                !preferencia.Recomendacoes)
            {
                continue;
            }

            foreach (var evento in eventos)
            {
                if (!EventoRules.PodeVisualizar(evento, aluno))
                    continue;
                if (!EventoCombinaComPreferencia(evento, preferencia, out var motivo))
                    continue;
                if (!jaProcessadas.Add(aluno.Id + ":" + evento.Id))
                    continue;

                var assunto = $"Evento recomendado - {evento.Nome}";
                var mensagem = EmailTemplates.AlertaEvento(
                    aluno.Nome,
                    evento.Nome,
                    evento.DataEvento.ToString("dd/MM/yyyy HH:mm"),
                    motivo);

                var envio = await _emailService.SendAsync(aluno.Email, assunto, mensagem);
                _db.NotificacaoEvento.Add(new NotificacaoEvento
                {
                    AlunoId = aluno.Id,
                    EventoId = evento.Id,
                    Tipo = TipoAlertaEvento,
                    Assunto = assunto,
                    Mensagem = mensagem,
                    Enviada = envio.IsSuccess,
                    Erro = envio.IsFailure ? string.Join("; ", envio.Errors) : null
                });

                if (envio.IsSuccess) processados++;
                else falhas++;
            }
        }

        return (processados, falhas);
    }

    private static bool EventoCombinaComPreferencia(Evento evento, PreferenciaNotificacao preferencia, out string motivo)
    {
        var categorias = (preferencia.Categorias ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (categorias.Contains(evento.Categoria.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            motivo = $"Combina com seu interesse em {evento.Categoria}.";
            return true;
        }

        if (preferencia is { UsarLocalizacao: true, LatitudeAproximada: not null, LongitudeAproximada: not null } &&
            evento.Latitude.HasValue &&
            evento.Longitude.HasValue)
        {
            var distanciaKm = DistanciaKm(
                preferencia.LatitudeAproximada.Value,
                preferencia.LongitudeAproximada.Value,
                evento.Latitude.Value,
                evento.Longitude.Value);
            if (distanciaKm <= preferencia.RaioKm)
            {
                motivo = $"Está a {distanciaKm:0.#} km da sua localização aproximada.";
                return true;
            }
        }

        motivo = "Evento próximo na agenda.";
        return evento.DataEvento <= DateTime.UtcNow.AddDays(7);
    }

    private static double DistanciaKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double raioTerraKm = 6371;
        static double Rad(double value) => value * Math.PI / 180;
        var dLat = Rad(lat2 - lat1);
        var dLon = Rad(lon2 - lon1);
        var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2)) *
                Math.Pow(Math.Sin(dLon / 2), 2);
        return raioTerraKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
