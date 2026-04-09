using Unievent.Application.Common;
using Unievent.Application.Dtos.Endereco;

namespace Unievent.Application.Interfaces.Services;

public interface IEnderecoService
{
    public Task<ResultData<EnderecoResponse>> CriarEndereco(EnderecoRequest request);
    public Task<ResultData<EnderecoResponse>> AtualizarEndereco(int id, EnderecoUpdate update);
    public Task<Result> DeletarEndereco(int id);
    public Task<ResultData<IEnumerable<EnderecoResponse>>> ListarEnderecos();
    public Task<ResultData<EnderecoResponse>> ListarEnderecoById(int id);
}
