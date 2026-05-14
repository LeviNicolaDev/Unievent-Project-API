using Unievent.Application.Common;
using Unievent.Application.Dtos.Instituicao;

namespace Unievent.Application.Interfaces.Services;

public interface IInstituicaoService
{
    public Task<Result<InstituicaoResponse>> CriarInstituicao(InstituicaoRequest request);
    public Task<Result<InstituicaoResponse>> AtualizarInstituicao(int id, InstituicaoUpdate update);
    public Task<Result<bool>> DeletarInstituicao(int id);
    public Task<Result<IEnumerable<InstituicaoResponse>>> ListarInstituicoes();
    public Task<Result<InstituicaoResponse>> ListarInstituicaoById(int id);
}
