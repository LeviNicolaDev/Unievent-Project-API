using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Unievent.Application.Common;
using Unievent.Application.Configurations;
using Unievent.Application.Dtos.Automacoes;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Templates;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Infra.Services;

public class CertificadoAutomaticoService(
    IDbContextFactory<AppDbContext> factory,
    IEmailService emailService,
    ICertificadoPdfGenerator pdfGenerator,
    IOptions<CertificacaoSettings> options,
    ILogger<CertificadoAutomaticoService> logger) : ICertificadoAutomaticoService
{
    public async Task<Result<CertificadoAutomaticoResult>> ProcessarAposCheckInAsync(
        int participacaoId, CancellationToken cancellationToken = default)
    {
        // Contexto independente: uma instância HTTP/worker nunca grava dados de outro processamento.
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var agora = DateTime.UtcNow;
        var reserva = Guid.NewGuid();
        var maxTentativas = Math.Clamp(options.Value.MaxTentativas, 1, 20);
        var ate = agora.AddMinutes(Math.Clamp(options.Value.ReservaMinutos, 2, 60));
        var reservado = false;
        var envioIniciado = false;
        try
        {
            var participacao = await db.Participacao.AsNoTracking()
                .Include(p => p.Aluno)
                .Include(p => p.Evento).ThenInclude(e => e.Instituicao)
                .Include(p => p.Evento).ThenInclude(e => e.ResponsavelEvento)
                .FirstOrDefaultAsync(p => p.Id == participacaoId, cancellationToken);
            if (participacao is null)
                return Result<CertificadoAutomaticoResult>.Failure("Participação não encontrada");
            if (!participacao.PresencaConfirmada)
                return Result<CertificadoAutomaticoResult>.Failure("Presença ainda não confirmada");

            var certificado = await db.Certificado.AsNoTracking()
                .Where(c => c.EventoId == participacao.EventoId)
                .OrderByDescending(c => c.DataCertifcado).ThenByDescending(c => c.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (certificado is null && participacao.CertificadoPdf is null)
                return Result<CertificadoAutomaticoResult>.Success(new(false, participacao.CertificadoEmitido,
                    participacao.CertificadoEnviadoPorEmail, false, null));

            // Um processo que morreu após iniciar SMTP pode ter entregado a mensagem.
            // Não liberar essa reserva para reenvio automático, mesmo depois de expirar.
            await db.Participacao.Where(p => p.Id == participacaoId &&
                    p.StatusEnvioCertificado == StatusEnvioCertificado.Enviando &&
                    p.ProcessamentoCertificadoAteUtc <= agora)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.StatusEnvioCertificado, StatusEnvioCertificado.EnvioIncerto)
                    .SetProperty(p => p.ErroEnvioCertificadoEmail,
                        "Processamento interrompido durante envio; verificar no provedor antes de reenviar."), cancellationToken);

            await db.Participacao.Where(p => p.Id == participacaoId && !p.CertificadoEnviadoPorEmail &&
                    p.TentativasEnvioCertificado >= maxTentativas &&
                    (p.StatusEnvioCertificado == StatusEnvioCertificado.Pendente ||
                     p.StatusEnvioCertificado == StatusEnvioCertificado.FalhaTemporaria ||
                     p.StatusEnvioCertificado == StatusEnvioCertificado.Preparando && p.ProcessamentoCertificadoAteUtc <= agora))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.StatusEnvioCertificado, StatusEnvioCertificado.FalhaPermanente)
                    .SetProperty(p => p.ErroEnvioCertificadoEmail, "Limite de tentativas atingido; requer análise."), cancellationToken);

            var obtida = await db.Participacao.Where(p => p.Id == participacaoId && p.PresencaConfirmada &&
                    (!p.CertificadoEnviadoPorEmail || p.CertificadoPdf == null) &&
                    p.TentativasEnvioCertificado < maxTentativas &&
                    (p.ProximaTentativaCertificadoUtc == null || p.ProximaTentativaCertificadoUtc <= agora) &&
                    (p.StatusEnvioCertificado == StatusEnvioCertificado.Pendente ||
                     p.StatusEnvioCertificado == StatusEnvioCertificado.FalhaTemporaria ||
                     p.StatusEnvioCertificado == StatusEnvioCertificado.Preparando && p.ProcessamentoCertificadoAteUtc <= agora ||
                     p.StatusEnvioCertificado == StatusEnvioCertificado.Enviado && p.CertificadoPdf == null))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.StatusEnvioCertificado, StatusEnvioCertificado.Preparando)
                    .SetProperty(p => p.ProcessamentoCertificadoId, reserva)
                    .SetProperty(p => p.ProcessamentoCertificadoAteUtc, ate)
                    .SetProperty(p => p.TentativasEnvioCertificado, p => p.TentativasEnvioCertificado + 1), cancellationToken);
            if (obtida == 0) return await ResultadoAsync(db, participacaoId, cancellationToken);
            reservado = true;

            // Releitura depois da reserva; evita usar um código/PDF anterior a uma chamada concorrente.
            participacao = await db.Participacao.AsNoTracking()
                .Include(p => p.Aluno)
                .Include(p => p.Evento).ThenInclude(e => e.Instituicao)
                .Include(p => p.Evento).ThenInclude(e => e.ResponsavelEvento)
                .SingleAsync(p => p.Id == participacaoId, cancellationToken);
            var instituicao = participacao.Evento.Instituicao?.Nome ??
                              participacao.Evento.Instituicao?.NomeAbreviado ?? "Instituição organizadora";
            var codigo = participacao.CodigoValidacao ?? Guid.NewGuid().ToString("N");
            if (participacao.CertificadoPdf is null)
            {
                var arquivo = pdfGenerator.Gerar(new CertificadoPdfDados(
                    participacao.Aluno.Nome, participacao.Evento.Nome, instituicao,
                    participacao.Evento.DataEvento, certificado!.Texto, codigo, agora,
                    participacao.Evento.ResponsavelEvento?.Nome));
                var gravado = await db.Participacao.Where(p => p.Id == participacaoId &&
                        p.ProcessamentoCertificadoId == reserva && p.StatusEnvioCertificado == StatusEnvioCertificado.Preparando)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.CertificadoPdf, arquivo.Conteudo)
                        .SetProperty(p => p.NomeArquivoCertificado, arquivo.NomeArquivo)
                        .SetProperty(p => p.DataGeracaoCertificado, agora)
                        .SetProperty(p => p.DestinatarioCertificadoEmail, participacao.Aluno.Email)
                        .SetProperty(p => p.CertificadoEmitido, true)
                        .SetProperty(p => p.CodigoValidacao, codigo), cancellationToken);
                if (gravado == 0) return await ResultadoAsync(db, participacaoId, cancellationToken);
            }

            // Compatibilidade: PDFs de emissões antigas podem ser materializados sem reenviar e-mails.
            if (participacao.CertificadoEnviadoPorEmail)
            {
                await db.Participacao.Where(p => p.Id == participacaoId && p.ProcessamentoCertificadoId == reserva)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.StatusEnvioCertificado, StatusEnvioCertificado.Enviado)
                        .SetProperty(p => p.ProcessamentoCertificadoId, (Guid?)null)
                        .SetProperty(p => p.ProcessamentoCertificadoAteUtc, (DateTime?)null), cancellationToken);
                return await ResultadoAsync(db, participacaoId, cancellationToken);
            }

            var arquivoSalvo = await db.Participacao.AsNoTracking().Where(p => p.Id == participacaoId)
                .Select(p => new { p.CertificadoPdf, p.NomeArquivoCertificado, p.DestinatarioCertificadoEmail })
                .SingleAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var podeEnviar = await db.Participacao.Where(p => p.Id == participacaoId &&
                    p.ProcessamentoCertificadoId == reserva && p.StatusEnvioCertificado == StatusEnvioCertificado.Preparando &&
                    !p.CertificadoEnviadoPorEmail && p.CertificadoPdf != null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.StatusEnvioCertificado, StatusEnvioCertificado.Enviando)
                    .SetProperty(p => p.ProcessamentoCertificadoAteUtc, ate), cancellationToken);
            if (podeEnviar == 0) return await ResultadoAsync(db, participacaoId, cancellationToken);
            envioIniciado = true;

            var envio = await emailService.SendWithAttachmentAsync(
                arquivoSalvo.DestinatarioCertificadoEmail!, $"Seu certificado do evento {participacao.Evento.Nome}",
                EmailTemplates.CertificadoDisponivel(participacao.Aluno.Nome, participacao.Evento.Nome,
                    "Obrigado por participar!", codigo, instituicao),
                new EmailAnexo(arquivoSalvo.CertificadoPdf!, arquivoSalvo.NomeArquivoCertificado!),
                $"<certificado-{participacaoId}-{codigo}@unievent.local>", cancellationToken);
            var status = envio.Situacao switch
            {
                SituacaoEnvioEmail.Enviado => StatusEnvioCertificado.Enviado,
                SituacaoEnvioEmail.FalhaTemporaria when participacao.TentativasEnvioCertificado < maxTentativas
                    => StatusEnvioCertificado.FalhaTemporaria,
                SituacaoEnvioEmail.Incerto => StatusEnvioCertificado.EnvioIncerto,
                _ => StatusEnvioCertificado.FalhaPermanente
            };
            // Não usar o cancelamento da requisição depois de SMTP aceitar: salvar o resultado é obrigatório.
            await FinalizarAsync(db, participacaoId, reserva, status, envio.Erro,
                ProximaTentativa(participacao.TentativasEnvioCertificado), CancellationToken.None);
            return await ResultadoAsync(db, participacaoId, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha na certificação da participação {ParticipacaoId}; presença preservada", participacaoId);
            if (reservado)
            {
                try
                {
                    var tentativas = await db.Participacao.Where(p => p.Id == participacaoId)
                        .Select(p => p.TentativasEnvioCertificado).SingleAsync(CancellationToken.None);
                    await FinalizarAsync(db, participacaoId, reserva,
                        envioIniciado ? StatusEnvioCertificado.EnvioIncerto :
                        tentativas >= maxTentativas ? StatusEnvioCertificado.FalhaPermanente : StatusEnvioCertificado.FalhaTemporaria,
                        envioIniciado ? "Resultado do envio incerto; verificar no provedor antes de reenviar."
                            : "Não foi possível preparar o certificado; presença preservada.",
                        ProximaTentativa(tentativas), CancellationToken.None);
                }
                catch (Exception persistenceError)
                {
                    // A reserva persistida permite recuperação segura pelo worker quando o banco retornar.
                    logger.LogError(persistenceError, "Falha ao registrar resultado da certificação {ParticipacaoId}", participacaoId);
                }
            }
            return Result<CertificadoAutomaticoResult>.Failure("Certificação pendente; presença confirmada foi preservada.");
        }
    }

    private DateTime ProximaTentativa(int tentativas) => DateTime.UtcNow.AddMinutes(
        Math.Min(60, Math.Clamp(options.Value.RetryMinutos, 1, 60) * Math.Pow(2, Math.Min(tentativas - 1, 6))));

    private static Task<int> FinalizarAsync(AppDbContext db, int id, Guid reserva, StatusEnvioCertificado status,
        string? erro, DateTime proxima, CancellationToken cancellationToken)
    {
        var enviado = status == StatusEnvioCertificado.Enviado;
        var mensagem = erro is null ? null : erro[..Math.Min(erro.Length, 500)];
        return db.Participacao.Where(p => p.Id == id && p.ProcessamentoCertificadoId == reserva && !p.CertificadoEnviadoPorEmail)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.StatusEnvioCertificado, status)
                .SetProperty(p => p.CertificadoEnviadoPorEmail, enviado)
                .SetProperty(p => p.DataEnvioCertificadoEmail, enviado ? DateTime.UtcNow : (DateTime?)null)
                .SetProperty(p => p.ErroEnvioCertificadoEmail, enviado ? null : mensagem)
                .SetProperty(p => p.ProximaTentativaCertificadoUtc,
                    status == StatusEnvioCertificado.FalhaTemporaria ? proxima : (DateTime?)null)
                .SetProperty(p => p.ProcessamentoCertificadoId, (Guid?)null)
                .SetProperty(p => p.ProcessamentoCertificadoAteUtc, (DateTime?)null), cancellationToken);
    }

    private static async Task<Result<CertificadoAutomaticoResult>> ResultadoAsync(AppDbContext db, int id, CancellationToken ct)
    {
        var result = await db.Participacao.AsNoTracking().Where(p => p.Id == id)
            .Select(p => new CertificadoAutomaticoResult(true, p.CertificadoEmitido, p.CertificadoEnviadoPorEmail,
                !p.CertificadoEnviadoPorEmail, p.ErroEnvioCertificadoEmail)).SingleAsync(ct);
        return Result<CertificadoAutomaticoResult>.Success(result);
    }
}
