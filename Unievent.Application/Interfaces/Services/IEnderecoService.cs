namespace Unievent.Application.Interfaces.Services;

public interface IEnderecoService
{
    public Task<EnderecoResponse> CriarEndereco(EnderecoRequest request);
    public Task<EnderecoResponse> AtualizarEndereco(int id, EnderecoUpdate update);
    public Task<bool> DeletarEndereco(int id);
    public Task<IList<EnderecoResponse>> ListarEnderecos();
    public Task<EnderecoResponse> ListarEnderecoById(int id);
}
