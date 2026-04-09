using Unievent.Application.Common;
using Unievent.Application.Dtos.Instituicao;

namespace Unievent.Application.Interfaces.Services;

public interface IInstituicaoService
{
    public Task<ResultData<InstituicaoResponse>> CriarInstituicao(InstituicaoRequest request);
    public Task<ResultData<InstituicaoResponse>> AtualizarInstituicao(int id, InstituicaoUpdate update);
    public Task<Result> DeletarInstituicao(int id);
    public Task<ResultData<IEnumerable<InstituicaoResponse>>> ListarInstituicoes();
    public Task<ResultData<InstituicaoResponse>> ListarInstituicaoById(int id);
}
