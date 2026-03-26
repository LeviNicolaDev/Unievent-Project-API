namespace Unievent.Application.Interfaces.Services;

public interface IInstituicaoService
{
    public Task<InstituicaoResponse> CriarInstituicao(InstituicaoRequest request);
    public Task<InstituicaoResponse> AtualizarInstituicao(int id, InstituicaoUpdate update);
    public Task<bool> DeletarInstituicao(int id);
    public Task<IList<InstituicaoResponse>> ListarInstituicoes();
    public Task<InstituicaoResponse> ListarInstituicaoById(int id);
}
