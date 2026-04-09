using Unievent.Application.Common;
using Unievent.Application.Dtos.ResponsavelEvento;

namespace Unievent.Application.Interfaces.Services;

public interface IResponsavelEventoService
{
    public Task<ResultData<ResponsavelEventoResponse>> CriarResponsavelEvento(ResponsavelEventoRequest responsavel);
    public Task<ResultData<ResponsavelEventoResponse>> ListarResponsavelEventoById(int id);
    public Task<ResultData<IEnumerable<ResponsavelEventoResponse>>> ListarResponsaveisEvento();
    public Task<ResultData<ResponsavelEventoResponse>> AtualizarResponsavelEvento(int id, ResponsavelEventoUpdate responsavel);
    public Task<Result> DeletarResponsavelEvento(int id);
}
