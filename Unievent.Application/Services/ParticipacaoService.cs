using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Participacao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services;

public class ParticipacaoService : IParticipacaoService
{
    private readonly IEventoRepository _repositoryEvento;
    private readonly IParticipacaoRepository _repositoryParticipacao;
    private readonly ILogger<ParticipacaoService> _logger;
    private readonly IAlunoRepository _repositoryAluno;
    public ParticipacaoService(IEventoRepository repositoryEvento, IParticipacaoRepository repositoryParticipacao, IAlunoRepository repositoryAluno, ILogger<ParticipacaoService> logger)
    {

        _repositoryEvento = repositoryEvento;
        _repositoryParticipacao = repositoryParticipacao;
        _repositoryAluno = repositoryAluno;
        _logger = logger;
    }


    async Task<Result<ParticipacaoResponse>> IParticipacaoService.EmitirCertificadoAsync(int alunoId, int eventoId)
    {
        throw new NotImplementedException();
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
        if (await _repositoryParticipacao.ExistsAsync(alunoId, eventoId))
        {
            return Result<ParticipacaoResponse>.Failure("Aluno já inscrito nesse evento");
        }
        var participacao = new Participacao(alunoId, eventoId);
        await _repositoryParticipacao.AddAsync(participacao);
        await _repositoryParticipacao.SaveChangesAsync();
        return Result<ParticipacaoResponse>.Success(new ParticipacaoResponse
        {
            Id = participacao.Id,
            NomeAluno = aluno.Nome,
            AlunoId = participacao.AlunoId,
            DataConfirmacao = participacao.DataConfirmacao,
            EventoId = participacao.EventoId,
            PresencaGarantida = participacao.PresencaConfirmada,
            CertificadoEmitido = participacao.CertificadoEmitido
        });

    }
}
