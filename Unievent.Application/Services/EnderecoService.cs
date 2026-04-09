using Unievent.Application.Common;
using Unievent.Application.Dtos.Endereco;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services;

public class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _repository;
    public EnderecoService(IEnderecoRepository repository)
    {
        _repository = repository;
    }
    async Task<ResultData<EnderecoResponse>> IEnderecoService.AtualizarEndereco(int id, EnderecoUpdate request)
    {
        var enderecoAntigo = await _repository.ListarEnderecoById(id);
        if (enderecoAntigo is null)
        {
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
    async Task<ResultData<EnderecoResponse>> IEnderecoService.CriarEndereco(EnderecoRequest request)
    {
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



    async Task<Result> IEnderecoService.DeletarEndereco(int id)
    {
        var endereco = await _repository.ListarEnderecoById(id);
        if (endereco is null)
        {
            return Result.Failure("Endereco não encontrado");


        }
        await _repository.DeletarEndereco(endereco);
        await _repository.SaveChangesAsync();
        return Result.Success("Endereco deletado com sucesso");
    }


    async Task<ResultData<EnderecoResponse>> IEnderecoService.ListarEnderecoById(int id)
    {
        var endereco = await _repository.ListarEnderecoById(id);
        if (endereco is null)
        {
            return ResultData<EnderecoResponse>.Failure("Endereco não encontrado");

        }
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

    async Task<ResultData<IEnumerable<EnderecoResponse>>> IEnderecoService.ListarEnderecos()
    {
        var enderecos = await _repository.ListarEnderecos();
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
}
