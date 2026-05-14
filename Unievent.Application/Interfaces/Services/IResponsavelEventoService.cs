using Unievent.Application.Common;
using Unievent.Application.Dtos.ResponsavelEvento;

namespace Unievent.Application.Interfaces.Services;

public interface IResponsavelEventoService
{
    public Task<Result<ResponsavelEventoResponse>> CriarResponsavelEvento(ResponsavelEventoRequest responsavel);
    public Task<Result<ResponsavelEventoResponse>> ListarResponsavelEventoById(int id);
    public Task<Result<IEnumerable<ResponsavelEventoResponse>>> ListarResponsaveisEvento();
    public Task<Result<ResponsavelEventoResponse>> AtualizarResponsavelEvento(int id, ResponsavelEventoUpdate responsavel);
    public Task<Result<bool>> DeletarResponsavelEvento(int id);
}
