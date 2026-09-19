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

        return Result<IngressoResponse>.Success(MapIngresso(participacao));
    }

    async Task<Result<IReadOnlyCollection<IngressoResponse>>> IParticipacaoService.ListarIngressosAsync(int alunoId)
    {
        var participacoes = await _repositoryParticipacao.ListByAlunoIdAsync(alunoId);
        return Result<IReadOnlyCollection<IngressoResponse>>.Success(
            participacoes.Select(MapIngresso).ToList());
    }

    async Task<Result<ParticipacaoResponse>> IParticipacaoService.ValidarCheckInAsync(int operadorId, CheckInRequest request)
    {
        var codigoIngresso = request.CodigoIngresso.Trim();
        var participacao = await _repositoryParticipacao.GetByCodigoIngressoAsync(codigoIngresso);
        if (participacao is null)
            return Result<ParticipacaoResponse>.Failure("Ingresso inválido");
        if (request.EventoId.HasValue && participacao.EventoId != request.EventoId.Value)
            return Result<ParticipacaoResponse>.Failure("Ingresso não pertence ao evento informado");
        if (participacao.StatusInscricao == StatusInscricao.Cancelada)
            return Result<ParticipacaoResponse>.Failure("Inscrição cancelada");
        if (participacao.PresencaConfirmada)
            return Result<ParticipacaoResponse>.Success(Map(participacao));

        var evento = participacao.Evento;
        var agora = DateTime.UtcNow;
        // O formulário cadastra horário civil de São Paulo; datetime2 não conserva DateTime.Kind.
        var dataUtc = evento.DataEvento.Kind == DateTimeKind.Unspecified
            ? TimeZoneInfo.ConvertTimeToUtc(evento.DataEvento, TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"))
            : evento.DataEvento.ToUniversalTime();
        var inicio = dataUtc.AddMinutes(-evento.ToleranciaCheckInMinutos);
        var fim = dataUtc.AddMinutes(evento.ToleranciaCheckInMinutos);
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

        var primeiraConfirmacao = await _repositoryParticipacao.TryConfirmarPresencaAsync(participacao);
        if (primeiraConfirmacao)
            await ProcessarCertificacaoSeguraAsync(participacao.Id);

        participacao = await _repositoryParticipacao.GetByCodigoIngressoAsync(codigoIngresso) ?? participacao;
        if (!primeiraConfirmacao && !participacao.PresencaConfirmada)
        {
            return participacao.StatusInscricao == StatusInscricao.Cancelada
                ? Result<ParticipacaoResponse>.Failure("Inscrição cancelada")
                : Result<ParticipacaoResponse>.Failure("Não foi possível confirmar o check-in");
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
        StatusInscricao = participacao.StatusInscricao,
        CertificadoEmitido = participacao.CertificadoEmitido,
        CertificadoEnviadoPorEmail = participacao.CertificadoEnviadoPorEmail,
        ErroEnvioCertificadoEmail = participacao.ErroEnvioCertificadoEmail,
        StatusEnvioCertificado = participacao.StatusEnvioCertificado,
        CertificadoPdfDisponivel = participacao.CertificadoPdf != null,
        DataConfirmacao = participacao.DataConfirmacao
    };

    private static IngressoResponse MapIngresso(Participacao participacao) => new()
    {
        Id = participacao.Id,
        EventoId = participacao.EventoId,
        NomeEvento = participacao.Evento?.Nome ?? string.Empty,
        InstituicaoNome = participacao.Evento?.Instituicao?.Nome ??
                          participacao.Evento?.Instituicao?.NomeAbreviado ??
                          "Instituição não informada",
        DataEvento = participacao.Evento?.DataEvento ?? default,
        Local = participacao.Evento?.Local ?? "Local não informado",
        StatusInscricao = participacao.StatusInscricao,
        PresencaConfirmada = participacao.PresencaConfirmada,
        DataCheckIn = participacao.DataConfirmacao,
        CodigoIngresso = participacao.CodigoIngresso,
        EventoRealizado = EventoJaRealizado(participacao.Evento?.DataEvento)
    };

    private static bool EventoJaRealizado(DateTime? dataEvento)
    {
        if (!dataEvento.HasValue) return false;
        var dataUtc = dataEvento.Value.Kind == DateTimeKind.Unspecified
            ? TimeZoneInfo.ConvertTimeToUtc(dataEvento.Value,
                TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"))
            : dataEvento.Value.ToUniversalTime();
        return dataUtc < DateTime.UtcNow;
    }

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
        if (_certificadoAutomaticoService is null)
            return Result<ParticipacaoResponse>.Failure("Serviço de certificação indisponível.");
        var resultado = await _certificadoAutomaticoService.ProcessarAposCheckInAsync(participacao.Id);
        if (resultado.IsFailure) return Result<ParticipacaoResponse>.Failure(resultado.Errors);
        if (resultado.Value?.CertificadoConfigurado != true)
            return Result<ParticipacaoResponse>.Failure("Evento sem certificado configurado.");
        participacao = await _repositoryParticipacao.GetByAlunoIdAndEventoIdAsync(alunoId, eventoId) ?? participacao;
        return Result<ParticipacaoResponse>.Success(Map(participacao));
    }

    private async Task ProcessarCertificacaoSeguraAsync(int participacaoId)
    {
        if (_certificadoAutomaticoService is null) return;
        try
        {
            var resultado = await _certificadoAutomaticoService.ProcessarAposCheckInAsync(participacaoId);
            if (resultado.IsFailure)
                _logger.LogWarning("Certificação pendente para participação {ParticipacaoId}; presença preservada", participacaoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na certificação da participação {ParticipacaoId}; presença preservada", participacaoId);
        }
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
            StatusInscricao = participacao.StatusInscricao,
            CertificadoEmitido = participacao.CertificadoEmitido,
            CertificadoEnviadoPorEmail = participacao.CertificadoEnviadoPorEmail,
            ErroEnvioCertificadoEmail = participacao.ErroEnvioCertificadoEmail
        });

    }
}
