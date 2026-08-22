using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Automacoes;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Templates;
using Unievent.Infra.Data;

namespace Unievent.Infra.Services;

public class CertificadoAutomaticoService : ICertificadoAutomaticoService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<CertificadoAutomaticoService> _logger;

    public CertificadoAutomaticoService(
        AppDbContext db,
        IEmailService emailService,
        ILogger<CertificadoAutomaticoService> logger)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<CertificadoAutomaticoResult>> ProcessarAposCheckInAsync(
        int participacaoId,
        CancellationToken cancellationToken = default)
    {
        var participacao = await _db.Participacao
            .Include(p => p.Aluno)
            .Include(p => p.Evento)
                .ThenInclude(e => e.Instituicao)
            .FirstOrDefaultAsync(p => p.Id == participacaoId, cancellationToken);

        if (participacao is null)
            return Result<CertificadoAutomaticoResult>.Failure("Participação não encontrada");
        if (!participacao.PresencaConfirmada)
            return Result<CertificadoAutomaticoResult>.Failure("Presença ainda não confirmada");

        var certificado = await _db.Certificado
            .AsNoTracking()
            .Where(c => c.EventoId == participacao.EventoId)
            .OrderByDescending(c => c.DataCertifcado)
            .FirstOrDefaultAsync(cancellationToken);

        if (certificado is null)
        {
            _logger.LogInformation(
                "Presença confirmada para participação {ParticipacaoId}, mas evento {EventoId} não possui certificado configurado",
                participacao.Id,
                participacao.EventoId);
            return Result<CertificadoAutomaticoResult>.Success(
                new CertificadoAutomaticoResult(false, false, false, false, null));
        }

        if (!participacao.CertificadoEmitido)
        {
            participacao.EmitirCertificado(Guid.NewGuid().ToString("N"));
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Certificado emitido automaticamente para aluno {AlunoId} no evento {EventoId}",
                participacao.AlunoId,
                participacao.EventoId);
        }

        if (participacao.CertificadoEnviadoPorEmail)
        {
            _logger.LogInformation(
                "Envio de certificado já realizado para participação {ParticipacaoId}; tentativa duplicada ignorada",
                participacao.Id);
            return Result<CertificadoAutomaticoResult>.Success(
                new CertificadoAutomaticoResult(true, true, true, false, null));
        }

        var codigo = participacao.CodigoValidacao ?? string.Empty;
        var instituicaoNome = participacao.Evento.Instituicao?.Nome ??
                              participacao.Evento.Instituicao?.NomeAbreviado ??
                              "instituição organizadora";
        var envio = await _emailService.SendAsync(
            participacao.Aluno.Email,
            $"Certificado disponível - {participacao.Evento.Nome}",
            EmailTemplates.CertificadoDisponivel(
                participacao.Aluno.Nome,
                participacao.Evento.Nome,
                certificado.Texto,
                codigo,
                instituicaoNome));

        if (envio.IsFailure)
        {
            var erro = string.Join("; ", envio.Errors);
            participacao.RegistrarFalhaEnvioCertificadoEmail(erro);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogWarning(
                "Certificado emitido para participação {ParticipacaoId}, mas o envio por e-mail ficou pendente: {Erro}",
                participacao.Id,
                erro);

            return Result<CertificadoAutomaticoResult>.Failure(erro);
        }

        participacao.RegistrarEnvioCertificadoEmail();
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "E-mail do certificado enviado para aluno {AlunoId} no evento {EventoId}",
            participacao.AlunoId,
            participacao.EventoId);

        return Result<CertificadoAutomaticoResult>.Success(
            new CertificadoAutomaticoResult(true, true, true, false, null));
    }
}
