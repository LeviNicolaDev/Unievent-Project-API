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

    async Task<Result<CertificadoResponse>> ICertificadoService.AtualizarCertificado(int id, CertificadoUpdate update)
    {
        try
        {
            var resultValidation = await _updateValidator.ValidateAsync(update);
            if (!resultValidation.IsValid)
            {
                _logger.LogWarning("Dados inválidos para atualização do certificado com ID {CertificadoId}", id);
                return Result<CertificadoResponse>.Failure(resultValidation.Errors.Select(e => e.ErrorMessage).ToList());
            }
            _logger.LogInformation("Iniciando atualização do certificado com ID {CertificadoId}", id);
            var certificado = await _repository.ListarCertificadoById(id);
            if (certificado is null)
            {
                _logger.LogWarning("Certificado com ID {CertificadoId} não encontrado para atualização", id);
                return Result<CertificadoResponse>.Failure("Certificado não encontrado");
            }
            if (!string.IsNullOrWhiteSpace(update.Texto))
            {
                certificado.Texto = update.Texto;
            }
            if (update.DataCertifcado.HasValue)
            {
                certificado.DataCertifcado = update.DataCertifcado.Value;
            }

            Evento? eventoAtualizado = null;
            if (update.EventoId.HasValue)
            {
                eventoAtualizado = await _eventoRepository.ListarEventoById(update.EventoId.Value);
                if (eventoAtualizado == null)
                {
                    _logger.LogWarning("Evento com ID {EventoId} não encontrado para atualização do certificado com ID {CertificadoId}", update.EventoId, id);
                    return Result<CertificadoResponse>.Failure("Evento não encontrado");
                }
                certificado.EventoId = update.EventoId.Value;
            }

            await _repository.AtualizarCertificado(certificado);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Certificado com ID {CertificadoId} atualizado com sucesso", id);
            return Result<CertificadoResponse>.Success(new CertificadoResponse
            {
                Id = id,
                DataCertifcado = certificado.DataCertifcado,
                EventoId = certificado.EventoId,
                NomeEvento = eventoAtualizado?.Nome ?? certificado.Evento?.Nome ?? string.Empty,
                Texto = certificado.Texto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar certificado com ID {CertificadoId}", id);
            return Result<CertificadoResponse>.Failure($"Erro ao atualizar certificado");
        }
    }


    async Task<Result<CertificadoResponse>> ICertificadoService.CriarCertificado(CertificadoRequest request)
    {
        try
        {
            var validationResult = await _requestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Dados inválidos para criação do certificado com evento ID {EventoId}", request.EventoId);
                return Result<CertificadoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            var evento = await _eventoRepository.ListarEventoById(request.EventoId);
            if (evento is null)
            {
                _logger.LogWarning("Evento com ID {EventoId} não encontrado para criação do certificado", request.EventoId);
                return Result<CertificadoResponse>.Failure("Evento não encontrado");
            }

            var certificado = new Certificado
            {
                DataCertifcado = request.DataCertifcado,
                EventoId = request.EventoId,
                Texto = request.Texto
            };
            await _repository.CriarCertificado(certificado);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Certificado criado com sucesso com ID {CertificadoId}", certificado.Id);
            return Result<CertificadoResponse>.Success(new CertificadoResponse
            {
                Id = certificado.Id,
                DataCertifcado = certificado.DataCertifcado,
                EventoId = certificado.EventoId,
                NomeEvento = evento.Nome,
                Texto = certificado.Texto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar certificado");
            return Result<CertificadoResponse>.Failure("Erro ao criar certificado");
        }
    }

    async Task<Result<bool>> ICertificadoService.DeletarCertificado(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção do certificado com ID {CertificadoId}", id);
            var certificado = await _repository.ListarCertificadoById(id);
            if (certificado is null)
            {
                _logger.LogWarning("Certificado com ID {CertificadoId} não encontrado para deleção", id);
                return Result<bool>.Failure("Certificado não encontrado");
            }
            await _repository.DeletarCertificado(certificado);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Certificado com ID {CertificadoId} deletado com sucesso", id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar certificado com ID {CertificadoId}", id);
            return Result<bool>.Failure("Erro ao deletar certificado");
        }
    }

    async Task<Result<CertificadoResponse>> ICertificadoService.ListarCertificadoById(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando busca do certificado com ID {CertificadoId}", id);
            var certificado = await _repository.ListarCertificadoById(id);
            if (certificado is null)
            {
                _logger.LogWarning("Certificado com ID {CertificadoId} não encontrado", id);
                return Result<CertificadoResponse>.Failure("Certificado não encontrado");
            }
            _logger.LogInformation("Certificado com ID {CertificadoId} encontrado com sucesso", id);
            return Result<CertificadoResponse>.Success(new CertificadoResponse
            {
                Id = certificado.Id,
                DataCertifcado = certificado.DataCertifcado,
                EventoId = certificado.EventoId,
                Texto = certificado.Texto,
                NomeEvento = certificado.Evento?.Nome ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar certificado com ID {CertificadoId}", id);
            return Result<CertificadoResponse>.Failure("Erro ao buscar certificado");
        }
    }

    async Task<Result<IEnumerable<CertificadoResponse>>> ICertificadoService.ListarCertificados()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de certificados");
            var certificados = await _repository.ListarCertificados();
            _logger.LogInformation("Certificados listados com sucesso {CertificadosCount}", certificados.Count());
            return Result<IEnumerable<CertificadoResponse>>.Success(certificados.Select(certificado => new CertificadoResponse
            {
                Id = certificado.Id,
                DataCertifcado = certificado.DataCertifcado,
                EventoId = certificado.EventoId,
                Texto = certificado.Texto,
                NomeEvento = certificado.Evento?.Nome ?? string.Empty
            }));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar certificados");
            return Result<IEnumerable<CertificadoResponse>>.Failure("Erro ao listar certificados");
        }
    }
}
