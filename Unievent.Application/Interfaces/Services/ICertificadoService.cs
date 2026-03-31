using Unievent.Application.Dtos.Certificado;

namespace Unievent.Application.Interfaces.Services;

public interface ICertificadoService
{
    public Task<CertificadoResponse> CriarCertificado(CertificadoRequest request);
    public Task<CertificadoResponse> AtualizarCertificado(int id, CertificadoUpdate update);
    public Task<bool> DeletarCertificado(int id);
    public Task<IList<CertificadoResponse>> ListarCertificados();
    public Task<CertificadoResponse> ListarCertificadoById(int id);
}
