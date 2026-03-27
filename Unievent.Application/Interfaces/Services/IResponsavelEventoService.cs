using Unievent.Application.Dtos.ResponsavelEvento;

namespace Unievent.Application.Interfaces.Services;

public interface IResponsavelEventoService
{
    public Task<ResponsavelEventoResponse> CriarResponsavelEvento(ResponsavelEventoRequest responsavel);
    public Task<ResponsavelEventoResponse> ListarResponsavelEventoById(int id);
    public Task<IList<ResponsavelEventoResponse>> ListarResponsaveisEvento();
    public Task<ResponsavelEventoResponse> AtualizarResponsavelEvento(int id, ResponsavelEventoUpdate responsavel);
    public Task<bool> DeletarResponsavelEvento(int id);
}
