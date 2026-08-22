using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Participacao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Rules;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Services;

public class ParticipacaoService : IParticipacaoService
{
    private readonly IEventoRepository _repositoryEvento;
    private readonly IParticipacaoRepository _repositoryParticipacao;
    private readonly ILogger<ParticipacaoService> _logger;
    private readonly IAlunoRepository _repositoryAluno;
    private readonly ICertificadoAutomaticoService? _certificadoAutomaticoService;

    public ParticipacaoService(
        IEventoRepository repositoryEvento,
        IParticipacaoRepository repositoryParticipacao,
        IAlunoRepository repositoryAluno,
        ILogger<ParticipacaoService> logger,
        ICertificadoAutomaticoService? certificadoAutomaticoService = null)
    {

        _repositoryEvento = repositoryEvento;
        _repositoryParticipacao = repositoryParticipacao;
        _repositoryAluno = repositoryAluno;
        _logger = logger;
        _certificadoAutomaticoService = certificadoAutomaticoService;
    }

    async Task<Result<IngressoResponse>> IParticipacaoService.ObterIngressoAsync(int alunoId, int eventoId)
    {
        var participacao = await _repositoryParticipacao.GetByAlunoIdAndEventoIdAsync(alunoId, eventoId);
        if (participacao is null)
            return Result<IngressoResponse>.Failure("Inscrição não encontrada");

        return Result<IngressoResponse>.Success(new IngressoResponse(
            eventoId, participacao.CodigoIngresso, participacao.PresencaConfirmada));
    }

    async Task<Result<ParticipacaoResponse>> IParticipacaoService.ValidarCheckInAsync(int operadorId, CheckInRequest request)
    {
        var participacao = await _repositoryParticipacao.GetByCodigoIngressoAsync(request.CodigoIngresso);
        if (participacao is null)
            return Result<ParticipacaoResponse>.Failure("Ingresso inválido");
        if (request.EventoId.HasValue && participacao.EventoId != request.EventoId.Value)
            return Result<ParticipacaoResponse>.Failure("Ingresso não pertence ao evento informado");
        if (participacao.PresencaConfirmada)
            return Result<ParticipacaoResponse>.Success(Map(participacao));

        var evento = participacao.Evento;
        var agora = DateTime.UtcNow;
        var inicio = evento.DataEvento.ToUniversalTime().AddMinutes(-evento.ToleranciaCheckInMinutos);
        var fim = evento.DataEvento.ToUniversalTime().AddMinutes(evento.ToleranciaCheckInMinutos);
        if (agora < inicio || agora > fim)
            return Result<ParticipacaoResponse>.Failure("Check-in fora da janela permitida");

        if (request.Latitude.HasValue || request.Longitude.HasValue || request.PrecisaoMetros.HasValue)
        {
            if (!request.Latitude.HasValue || !request.Longitude.HasValue || !request.PrecisaoMetros.HasValue)
                return Result<ParticipacaoResponse>.Failure("Localização incompleta para validação");
            if (request.PrecisaoMetros.Value <= 0 || request.PrecisaoMetros.Value > 50)
                return Result<ParticipacaoResponse>.Failure("Localização imprecisa; aproxime-se do evento e tente novamente");
            if (!evento.Latitude.HasValue || !evento.Longitude.HasValue)
                return Result<ParticipacaoResponse>.Failure("Evento sem localização configurada para check-in por localização");

            var distancia = CalcularDistanciaMetros(
                request.Latitude.Value, request.Longitude.Value, evento.Latitude.Value, evento.Longitude.Value);
            if (distancia > evento.RaioCheckInMetros)
                return Result<ParticipacaoResponse>.Failure($"Fora do raio permitido ({Math.Round(distancia)} m)");

            participacao.ConfirmarPresenca(distancia, request.PrecisaoMetros.Value, operadorId);
        }
        else
        {
            participacao.ConfirmarPresencaPorCodigo(operadorId);
        }

        await _repositoryParticipacao.Update(participacao);
        await _repositoryParticipacao.SaveChangesAsync();

        if (_certificadoAutomaticoService is not null)
        {
            var certificado = await _certificadoAutomaticoService.ProcessarAposCheckInAsync(participacao.Id);
            if (certificado.IsFailure)
            {
                _logger.LogWarning(
                    "Presença confirmada para participação {ParticipacaoId}, mas a automação do certificado falhou: {Erros}",
                    participacao.Id,
                    string.Join("; ", certificado.Errors));
            }
        }

        return Result<ParticipacaoResponse>.Success(Map(participacao));
    }

    private static ParticipacaoResponse Map(Participacao participacao) => new()
    {
        Id = participacao.Id,
        AlunoId = participacao.AlunoId,
        NomeAluno = participacao.Aluno?.Nome ?? string.Empty,
        EventoId = participacao.EventoId,
        PresencaGarantida = participacao.PresencaConfirmada,
        CertificadoEmitido = participacao.CertificadoEmitido,
        CertificadoEnviadoPorEmail = participacao.CertificadoEnviadoPorEmail,
        ErroEnvioCertificadoEmail = participacao.ErroEnvioCertificadoEmail,
        DataConfirmacao = participacao.DataConfirmacao
    };

    private static double CalcularDistanciaMetros(double lat1, double lon1, double lat2, double lon2)
    {
        const double raioTerra = 6371000;
        static double Rad(double graus) => graus * Math.PI / 180;
        var dLat = Rad(lat2 - lat1);
        var dLon = Rad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return raioTerra * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }


    async Task<Result<ParticipacaoResponse>> IParticipacaoService.EmitirCertificadoAsync(int alunoId, int eventoId)
    {
        var participacao = await _repositoryParticipacao.GetByAlunoIdAndEventoIdAsync(alunoId, eventoId);
        if (participacao is null)
            return Result<ParticipacaoResponse>.Failure("Inscrição não encontrada para este evento.");
        if (!participacao.PresencaConfirmada)
            return Result<ParticipacaoResponse>.Failure("Aluno sem presença confirmada.");
        if (participacao.CertificadoEmitido)
            return Result<ParticipacaoResponse>.Success(Map(participacao));

        participacao.EmitirCertificado(Guid.NewGuid().ToString("N"));
        await _repositoryParticipacao.Update(participacao);
        await _repositoryParticipacao.SaveChangesAsync();

        return Result<ParticipacaoResponse>.Success(Map(participacao));
    }

    async Task<Result<ParticipacaoResponse>> IParticipacaoService.GarantirPresencaAsync(int alunoId, int eventoId)
    {
        var participacao = await _repositoryParticipacao.GetByAlunoIdAndEventoIdAsync(alunoId, eventoId);

        if (participacao is null)
        {
            _logger.LogWarning("Inscrição não encontrada para o aluno {AlunoId} e evento {EventoId}", alunoId, eventoId);
            return Result<ParticipacaoResponse>.Failure("Inscrição não encontrada para este evento.");
        }
        if (participacao.PresencaConfirmada)
        {
            _logger.LogWarning("Presença já confirmada para o aluno {AlunoId} e evento {EventoId}", alunoId, eventoId);
            return Result<ParticipacaoResponse>.Failure("Presença já confirmada.");
        }

        participacao.ConfirmarPresenca();

        await _repositoryParticipacao.Update(participacao);
        await _repositoryParticipacao.SaveChangesAsync();

        var response = new ParticipacaoResponse
        {
            Id = participacao.Id,
            AlunoId = participacao.AlunoId,
            NomeAluno = participacao.Aluno.Nome,
            EventoId = participacao.EventoId,
            PresencaGarantida = participacao.PresencaConfirmada,
            CertificadoEmitido = participacao.CertificadoEmitido,
            CertificadoEnviadoPorEmail = participacao.CertificadoEnviadoPorEmail,
            ErroEnvioCertificadoEmail = participacao.ErroEnvioCertificadoEmail,
            DataConfirmacao = participacao.DataConfirmacao
        };

        return Result<ParticipacaoResponse>.Success(response);
    }

    async Task<Result<ParticipacaoResponse>> IParticipacaoService.InscreverAsync(int alunoId, int eventoId)
    {
        var aluno = await _repositoryAluno.ListarAlunoById(alunoId);
        if (aluno is null)

        {
            return Result<ParticipacaoResponse>.Failure("Aluno não encontrado");
        }
        var evento = await _repositoryEvento.ListarEventoById(eventoId);
        if (evento is null)
        {
            return Result<ParticipacaoResponse>.Failure("Evento não encontrado");
        }
        var agora = DateTime.UtcNow;
        if (!EventoRules.InscricoesAbertas(evento, agora, out var erroInscricao))
            return Result<ParticipacaoResponse>.Failure(erroInscricao!);
        if (!EventoRules.PodeParticipar(evento, aluno))
        {
            if (evento.PublicoPermitido == PublicoPermitido.AlunosDaInstituicao &&
                aluno.TipoParticipante == TipoParticipante.Interno &&
                evento.InstituicaoId.HasValue &&
                aluno.InstituicaoId != evento.InstituicaoId)
            {
                var nomeInstituicao = evento.Instituicao?.Nome ??
                                      evento.Instituicao?.NomeAbreviado ??
                                      "instituição organizadora";
                return Result<ParticipacaoResponse>.Failure($"Este evento é exclusivo para alunos da {nomeInstituicao}.");
            }

            return Result<ParticipacaoResponse>.Failure("Participante sem permissão para este evento");
        }
        if (await _repositoryParticipacao.CountByEventoIdAsync(eventoId) >= evento.Capacidade)
            return Result<ParticipacaoResponse>.Failure("Evento lotado");
        if (await _repositoryParticipacao.ExistsAsync(alunoId, eventoId))
        {
            return Result<ParticipacaoResponse>.Failure("Aluno já inscrito nesse evento");
        }
        var participacao = new Participacao(alunoId, eventoId);
        if (!await _repositoryParticipacao.TryAddWithinCapacityAsync(participacao, evento.Capacidade))
            return Result<ParticipacaoResponse>.Failure("Evento lotado ou inscrição já existente");
        return Result<ParticipacaoResponse>.Success(new ParticipacaoResponse
        {
            Id = participacao.Id,
            NomeAluno = aluno.Nome,
            AlunoId = participacao.AlunoId,
            DataConfirmacao = participacao.DataConfirmacao,
            EventoId = participacao.EventoId,
            PresencaGarantida = participacao.PresencaConfirmada,
            CertificadoEmitido = participacao.CertificadoEmitido,
            CertificadoEnviadoPorEmail = participacao.CertificadoEnviadoPorEmail,
            ErroEnvioCertificadoEmail = participacao.ErroEnvioCertificadoEmail
        });

    }
}
