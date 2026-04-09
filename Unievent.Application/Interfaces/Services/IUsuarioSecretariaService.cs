using Unievent.Application.Common;
using Unievent.Application.Dtos.UsuarioSecretaria;

namespace Unievent.Application.Interfaces.Services;

public interface IUsuarioSecretariaService
{
    public Task<ResultData<UsuarioSecretariaResponse>> CriarUsuarioSecretaria(UsuarioSecretariaRequest request);
    public Task<ResultData<UsuarioSecretariaResponse>> AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update);
    public Task<Result> DeletarUsuarioSecretaria(int id);
    public Task<ResultData<IEnumerable<UsuarioSecretariaResponse>>> ListarUsuarioSecretaria();
    public Task<ResultData<UsuarioSecretariaResponse>> ListarUsuarioSecretariaById(int id);
    public Task<ResultData<UsuarioSecretariaLoginResponse>> Login(UsuarioSecretariaLoginRequest request);
}
