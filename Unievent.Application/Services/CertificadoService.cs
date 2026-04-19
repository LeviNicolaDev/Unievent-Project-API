using FluentValidation;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services;

public class CertificadoService : ICertificadoService
{
    private readonly ICertificadoRepository _repository;
    private readonly ILogger<CertificadoService> _logger;
    private readonly IValidator<CertificadoRequest> _requestValidator;
    private readonly IValidator<CertificadoUpdate> _updateValidator;
    private readonly IAlunoRepository _alunoRepository;
    private readonly IEventoRepository _eventoRepository;
    public CertificadoService(ICertificadoRepository repository, ILogger<CertificadoService> logger, IValidator<CertificadoRequest> requestValidator, 
    IValidator<CertificadoUpdate> updateValidator, IAlunoRepository alunoRepository, IEventoRepository eventoRepository)
    {

        _repository = repository;
        _logger = logger;
        _requestValidator = requestValidator;
        _updateValidator = updateValidator;
        _alunoRepository = alunoRepository;
        _eventoRepository = eventoRepository;
    }

    async Task<ResultData<CertificadoResponse>> ICertificadoService.AtualizarCertificado(int id, CertificadoUpdate update)
    {
        try
        {
            var resuultValidation = await _updateValidator.ValidateAsync(update);
            if (!resuultValidation.IsValid){
                _logger.LogWarning("Dados inválidos para atualização do certificado com ID {CertificadoId}", id);
                return ResultData<CertificadoResponse>.Failure("Dados inválidos");
            }
            _logger.LogInformation("Iniciando atualização do certificado com ID {CertificadoId}", id);
            var certificado = await _repository.ListarCertificadoById(id);
            if (certificado is null)
            {
                _logger.LogWarning("Certificado com ID {CertificadoId} não encontrado para atualização", id);
                return ResultData<CertificadoResponse>.Failure("Certificado não encontrado");
            }
            if (!string.IsNullOrWhiteSpace(update.Texto))
            {
                certificado.Texto = update.Texto;
            }
            if (update.DataCertifcado.HasValue)
            {
                certificado.DataCertifcado = update.DataCertifcado.Value;
            }
            if (update.AlunoId.HasValue)
            {
                 var aluno = await _alunoRepository.ListarAlunoById(update.AlunoId.Value);

                if (aluno == null)
                {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado...", update.AlunoId);
                return ResultData<CertificadoResponse>.Failure("Aluno não encontrado");
                }   

                certificado.AlunoId = update.AlunoId.Value;
            }
            if (update.EventoId.HasValue)
            {
            var evento = await _eventoRepository.ListarEventoById(update.EventoId.Value);
            if(evento == null)
                {
                _logger.LogWarning("Evento com ID {EventoId} não encontrado para atualização do certificado com ID {CertificadoId}", update.EventoId, id);
                return ResultData<CertificadoResponse>.Failure("Evento não encontrado");       
                }
                certificado.EventoId = update.EventoId.Value;
            }
          
            await _repository.AtualizarCertificado(certificado);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Certificado com ID {CertificadoId} atualizado com sucesso", id);
            return ResultData<CertificadoResponse>.Success(new CertificadoResponse
            {
                Id = id,
                DataCertifcado = certificado.DataCertifcado,
                AlunoId = certificado.AlunoId,
                EventoId = certificado.EventoId,
                Texto = certificado.Texto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar certificado com ID {CertificadoId}", id);
            return ResultData<CertificadoResponse>.Failure($"Erro ao atualizar certificado");
        }
    }


    async Task<ResultData<CertificadoResponse>> ICertificadoService.CriarCertificado(CertificadoRequest request)
    {
        try
        {
            var validationResult = await _requestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)            {
                _logger.LogWarning("Dados inválidos para criação do certificado para aluno ID {AlunoId} e evento ID {EventoId}", request.AlunoId, request.EventoId);
                return ResultData<CertificadoResponse>.Failure("Dados inválidos");
            }
            _logger.LogInformation("Iniciando criação de certificado para aluno ID {AlunoId} e evento ID {EventoId}", request.AlunoId, request.EventoId);
            
            var aluno = await _alunoRepository.ListarAlunoById(request.AlunoId);
            if (aluno is null)            {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado para criação do certificado", request.AlunoId);
                return ResultData<CertificadoResponse>.Failure("Aluno não encontrado");
            }
            var evento = await _eventoRepository.ListarEventoById(request.EventoId);
            if (evento is null)            {
                _logger.LogWarning("Evento com ID {EventoId} não encontrado para criação do certificado", request.EventoId);
                return ResultData<CertificadoResponse>.Failure("Evento não encontrado");
            }   

            var certificado = new Certificado
            {
                DataCertifcado = request.DataCertifcado,
                AlunoId = request.AlunoId,
                EventoId = request.EventoId,
                Texto = request.Texto
            };
            await _repository.CriarCertificado(certificado);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Certificado criado com sucesso com ID {CertificadoId}", certificado.Id);
            return ResultData<CertificadoResponse>.Success(new CertificadoResponse
            {
                Id = certificado.Id,
                DataCertifcado = certificado.DataCertifcado,
                AlunoId = certificado.AlunoId,
                EventoId = certificado.EventoId,
                Texto = certificado.Texto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar certificado");
            return ResultData<CertificadoResponse>.Failure("Erro ao criar certificado");
        }
    }

    async Task<Result> ICertificadoService.DeletarCertificado(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção do certificado com ID {CertificadoId}", id);
            var certificado = await _repository.ListarCertificadoById(id);
            if (certificado is null)
            {
                _logger.LogWarning("Certificado com ID {CertificadoId} não encontrado para deleção", id);
                return Result.Failure("Certificado não encontrado");
            }
            await _repository.DeletarCertificado(certificado);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Certificado com ID {CertificadoId} deletado com sucesso", id);
            return Result.Success("Certificado deletado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar certificado com ID {CertificadoId}", id);
            return Result.Failure("Erro ao deletar certificado");
        }
    }

    async Task<ResultData<CertificadoResponse>> ICertificadoService.ListarCertificadoById(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando busca do certificado com ID {CertificadoId}", id);
            var certificado = await _repository.ListarCertificadoById(id);
            if (certificado is null)
            {
                _logger.LogWarning("Certificado com ID {CertificadoId} não encontrado", id);
                return ResultData<CertificadoResponse>.Failure("Certificado não encontrado");
            }
            _logger.LogInformation("Certificado com ID {CertificadoId} encontrado com sucesso", id);
            return ResultData<CertificadoResponse>.Success(new CertificadoResponse
            {
                Id = certificado.Id,
                DataCertifcado = certificado.DataCertifcado,
                AlunoId = certificado.AlunoId,
                EventoId = certificado.EventoId,
                Texto = certificado.Texto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar certificado com ID {CertificadoId}", id);
            return ResultData<CertificadoResponse>.Failure("Erro ao buscar certificado");
        }
    }

    async Task<ResultData<IEnumerable<CertificadoResponse>>> ICertificadoService.ListarCertificados()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de certificados");
            var certificados = await _repository.ListarCertificados();
            _logger.LogInformation("Certificados listados com sucesso {CertificadosCount}", certificados.Count());
            return ResultData<IEnumerable<CertificadoResponse>>.Success(certificados.Select(certificado => new CertificadoResponse
            {
                Id = certificado.Id,
                DataCertifcado = certificado.DataCertifcado,
                AlunoId = certificado.AlunoId,
                EventoId = certificado.EventoId,
                Texto = certificado.Texto
            }));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar certificados");
            return ResultData<IEnumerable<CertificadoResponse>>.Failure("Erro ao listar certificados");
        }
    }
}
