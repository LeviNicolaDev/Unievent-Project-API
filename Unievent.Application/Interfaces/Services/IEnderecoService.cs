using Unievent.Application.Common;
using Unievent.Application.Dtos.Endereco;

namespace Unievent.Application.Interfaces.Services;

public interface IEnderecoService
{
    public Task<Result<EnderecoResponse>> CriarEndereco(EnderecoRequest request);
    public Task<Result<EnderecoResponse>> AtualizarEndereco(int id, EnderecoUpdate update);
    public Task<Result<bool>> DeletarEndereco(int id);
    public Task<Result<IEnumerable<EnderecoResponse>>> ListarEnderecos();
    public Task<Result<EnderecoResponse>> ListarEnderecoById(int id);
}
