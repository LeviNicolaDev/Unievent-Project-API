using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.ResponsavelEvento;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services
{
    public class ResponsavelEventoService : IResponsavelEventoService
    {
        private readonly IResponsavelEventoRepository _repository;
        private readonly ILogger<ResponsavelEventoService> _logger;
        private readonly IValidator<ResponsavelEventoRequest> _validatorRequest;
        private readonly IValidator<ResponsavelEventoUpdate> _validatorUpdate;
        public ResponsavelEventoService(IResponsavelEventoRepository repository, ILogger<ResponsavelEventoService> logger, IValidator<ResponsavelEventoRequest> validatorRequest, IValidator<ResponsavelEventoUpdate> validatorUpdate)
        {
            _repository = repository;
            _logger = logger;
            _validatorRequest = validatorRequest;
            _validatorUpdate = validatorUpdate;
        }
        async Task<ResultData<ResponsavelEventoResponse>> IResponsavelEventoService.AtualizarResponsavelEvento(int id, ResponsavelEventoUpdate responsavel)
        {
            try
            {
                var validationResult = await _validatorUpdate.ValidateAsync(responsavel);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Dados inválidos para atualização do responsável do evento com ID {ResponsavelId}", id);
                    return ResultData<ResponsavelEventoResponse>.Failure("Dados inválidos");
                }
                _logger.LogInformation("Iniciando atualização do responsável do evento com ID {ResponsavelId}", id);
                var responsavelAntigo = await _repository.ListarResponsavelEventoById(id);
                if (responsavelAntigo is null)
                {
                    _logger.LogWarning("Responsável com ID {ResponsavelId} não encontrado para atualização", id);
                    return ResultData<ResponsavelEventoResponse>.Failure("Responsável não encontrado");
                }
                if (!string.IsNullOrWhiteSpace(responsavel.Nome))
                {
                    responsavelAntigo.Nome = responsavel.Nome;
                }
                if (responsavel.FotoPerfil != null)
                {
                    var imagem = await SalvarImagem(responsavel.FotoPerfil);
                    responsavelAntigo.FotoPerfil = imagem;
                }
                await _repository.AtualizarResponsavelEvento(responsavelAntigo);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Responsável do evento com ID {ResponsavelId} atualizado com sucesso", id);
                return ResultData<ResponsavelEventoResponse>.Success(new ResponsavelEventoResponse
                {
                    Id = id,
                    Nome = responsavelAntigo.Nome,
                    FotoPerfil = responsavelAntigo.FotoPerfil
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar responsável do evento com ID {ResponsavelId}", id);
                return ResultData<ResponsavelEventoResponse>.Failure("Erro ao atualizar responsável do evento");
            }

        }

        async Task<ResultData<ResponsavelEventoResponse>> IResponsavelEventoService.CriarResponsavelEvento(ResponsavelEventoRequest responsavel)
        {
            try
            {
                var validationResult = await _validatorRequest.ValidateAsync(responsavel);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Dados inválidos para criação do responsável do evento com nome {ResponsavelNome}", responsavel.Nome);
                    return ResultData<ResponsavelEventoResponse>.Failure("Dados inválidos");
                }
                _logger.LogInformation("Iniciando criação do responsável do evento com nome {ResponsavelNome}", responsavel.Nome);
                var imagem = await SalvarImagem(responsavel.FotoPerfil);
                var responsavelNovo = new ResponsavelEvento
                {
                    Nome = responsavel.Nome,
                    FotoPerfil = imagem
                };
                await _repository.CriarResponsavelEvento(responsavelNovo);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Responsável do evento com nome {ResponsavelNome} criado com sucesso", responsavel.Nome);
                return ResultData<ResponsavelEventoResponse>.Success(new ResponsavelEventoResponse
                {
                    Id = responsavelNovo.Id,
                    Nome = responsavelNovo.Nome,
                    FotoPerfil = responsavelNovo.FotoPerfil
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar responsável do evento com nome {ResponsavelNome}", responsavel.Nome);
                return ResultData<ResponsavelEventoResponse>.Failure("Erro ao criar responsável do evento");
            }

        }

        async Task<Result> IResponsavelEventoService.DeletarResponsavelEvento(int id)
        {
            var responsavel = await _repository.ListarResponsavelEventoById(id);
            if (responsavel is null)
            {
                return Result.Failure("Responsável do evento não encontrado");
            }
            await _repository.DeletarResponsavelEvento(responsavel);
            await _repository.SaveChangesAsync();
            return Result.Success("Responsável do evento deletado com sucesso");
        }

        async Task<ResultData<IEnumerable<ResponsavelEventoResponse>>> IResponsavelEventoService.ListarResponsaveisEvento()
        {
            try
            {
                _logger.LogInformation("Iniciando listagem de responsáveis do evento");
                var responsaveis = await _repository.ListarResponsaveisEvento();
                _logger.LogInformation("Responsáveis do evento listados com sucesso {ResponsaveisCount}", responsaveis.Count());
                return ResultData<IEnumerable<ResponsavelEventoResponse>>.Success(responsaveis.Select(l => new ResponsavelEventoResponse
                {
                    Id = l.Id,
                    Nome = l.Nome,
                    FotoPerfil = l.FotoPerfil
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar responsáveis do evento");
                return ResultData<IEnumerable<ResponsavelEventoResponse>>.Failure("Erro ao listar responsáveis do evento");
            }


        }

        async Task<ResultData<ResponsavelEventoResponse>> IResponsavelEventoService.ListarResponsavelEventoById(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando busca do responsável do evento com ID {ResponsavelId}", id);
                var responsavel = await _repository.ListarResponsavelEventoById(id);
                if (responsavel == null)
                {
                    _logger.LogWarning("Responsável do evento com ID {ResponsavelId} não encontrado", id);
                    return ResultData<ResponsavelEventoResponse>.Failure("Responsável do evento não encontrado");
                }
                _logger.LogInformation("Responsável do evento com ID {ResponsavelId} encontrado com sucesso", id);
                return ResultData<ResponsavelEventoResponse>.Success(new ResponsavelEventoResponse
                {

                    Id = responsavel.Id,
                    Nome = responsavel.Nome,
                    FotoPerfil = responsavel.FotoPerfil
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar responsável do evento com ID {ResponsavelId}", id);
                return ResultData<ResponsavelEventoResponse>.Failure("Erro ao buscar responsável do evento");
            }
        }
        public async Task<string> SalvarImagem(IFormFile imagem)
        {
            var pasta = Path.Combine("wwwroot", "imagens");
            if (!Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }


            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
            var caminho = Path.Combine(pasta, nomeArquivo);

            using var stream = new FileStream(caminho, FileMode.Create);
            await imagem.CopyToAsync(stream);

            return $"/imagens/{nomeArquivo}";
        }
    }
}
