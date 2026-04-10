using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Endereco;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services;

public class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _repository;
    private readonly ILogger<EnderecoService> _logger;
    public EnderecoService(IEnderecoRepository repository, ILogger<EnderecoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    async Task<ResultData<EnderecoResponse>> IEnderecoService.AtualizarEndereco(int id, EnderecoUpdate request)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização do endereço com ID {EnderecoId}", id);
            var enderecoAntigo = await _repository.ListarEnderecoById(id);
            if (enderecoAntigo is null)
            {
                _logger.LogWarning("Endereço com ID {EnderecoId} não encontrado para atualização", id);
                return ResultData<EnderecoResponse>.Failure("Endereco não encontrado");
            }
            if (!string.IsNullOrWhiteSpace(request.Bairro))
            {
                enderecoAntigo.Bairro = request.Bairro;
            }
            if (!string.IsNullOrWhiteSpace(request.Cep))
            {
                enderecoAntigo.Cep = request.Cep;
            }
            if (!string.IsNullOrWhiteSpace(request.Cidade))
            {
                enderecoAntigo.Cidade = request.Cidade;
            }
            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                enderecoAntigo.Estado = request.Estado;
            }
            if (!string.IsNullOrWhiteSpace(request.Numero))
            {
                enderecoAntigo.Numero = request.Numero;
            }
            if (!string.IsNullOrWhiteSpace(request.Rua))
            {
                enderecoAntigo.Rua = request.Rua;
            }
            await _repository.AtualizarEndereco(enderecoAntigo);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Endereço com ID {EnderecoId} atualizado com sucesso", id);
            return ResultData<EnderecoResponse>.Success(new EnderecoResponse
            {
                Id = id,
                Bairro = enderecoAntigo.Bairro,
                Cep = enderecoAntigo.Cep,
                Cidade = enderecoAntigo.Cidade,
                Estado = enderecoAntigo.Estado,
                Numero = enderecoAntigo.Numero,
                Rua = enderecoAntigo.Rua
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar endereço com ID {EnderecoId}", id);
            return ResultData<EnderecoResponse>.Failure($"Erro ao atualizar endereço");
        }


    }
    async Task<ResultData<EnderecoResponse>> IEnderecoService.CriarEndereco(EnderecoRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de endereço para rua {Rua}, número {Numero}", request.Rua, request.Numero);
            var endereco = new Endereco
            {
                Bairro = request.Bairro,
                Cep = request.Cep,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Numero = request.Numero,
                Rua = request.Rua
            };
            await _repository.CriarEndereco(endereco);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Endereço criado com sucesso com ID {EnderecoId}", endereco.Id);
            return ResultData<EnderecoResponse>.Success(new EnderecoResponse
            {
                Id = endereco.Id,
                Bairro = endereco.Bairro,
                Cep = endereco.Cep,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Numero = endereco.Numero,
                Rua = endereco.Rua
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar endereço para rua {Rua}, número {Numero}", request.Rua, request.Numero);
            return ResultData<EnderecoResponse>.Failure("Erro ao criar endereço");
        }
    }



    async Task<Result> IEnderecoService.DeletarEndereco(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção do endereço com ID {EnderecoId}", id);
            var endereco = await _repository.ListarEnderecoById(id);
            if (endereco is null)
            {
                _logger.LogWarning("Endereço com ID {EnderecoId} não encontrado para deleção", id);
                return Result.Failure("Endereco não encontrado");


            }
            await _repository.DeletarEndereco(endereco);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Endereço com ID {EnderecoId} deletado com sucesso", id);
            return Result.Success("Endereco deletado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar endereço com ID {EnderecoId}", id);
            return Result.Failure("Erro ao deletar endereço");
        }
    }


    async Task<ResultData<EnderecoResponse>> IEnderecoService.ListarEnderecoById(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando busca do endereço com ID {EnderecoId}", id);
            var endereco = await _repository.ListarEnderecoById(id);
            if (endereco is null)
            {
                _logger.LogWarning("Endereço com ID {EnderecoId} não encontrado", id);
                return ResultData<EnderecoResponse>.Failure("Endereco não encontrado");

            }
            _logger.LogInformation("Endereço com ID {EnderecoId} encontrado com sucesso", id);
            return ResultData<EnderecoResponse>.Success(new EnderecoResponse
            {
                Id = endereco.Id,
                Bairro = endereco.Bairro,
                Cep = endereco.Cep,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Numero = endereco.Numero,
                Rua = endereco.Rua
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar endereço com ID {EnderecoId}", id);
            return ResultData<EnderecoResponse>.Failure("Erro ao buscar endereço");
        }
    }

    async Task<ResultData<IEnumerable<EnderecoResponse>>> IEnderecoService.ListarEnderecos()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de endereços");
            var enderecos = await _repository.ListarEnderecos();
            _logger.LogInformation("Endereços listados com sucesso {EnderecosCount}", enderecos.Count());
            return ResultData<IEnumerable<EnderecoResponse>>.Success(enderecos.Select(e => new EnderecoResponse
            {
                Id = e.Id,
                Bairro = e.Bairro,
                Cep = e.Cep,
                Cidade = e.Cidade,
                Estado = e.Estado,
                Numero = e.Numero,
                Rua = e.Rua
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar endereços");
            return ResultData<IEnumerable<EnderecoResponse>>.Failure("Erro ao listar endereços");
        }

    }
}
