using Unievent.Application.Common;
using Unievent.Application.Dtos.Certificado;

namespace Unievent.Application.Interfaces.Services;

public interface ICertificadoService
{
    public Task<Result<CertificadoResponse>> CriarCertificado(CertificadoRequest request);
    public Task<Result<CertificadoResponse>> AtualizarCertificado(int id, CertificadoUpdate update);
    public Task<Result<bool>> DeletarCertificado(int id);
    public Task<Result<IEnumerable<CertificadoResponse>>> ListarCertificados();
    public Task<Result<CertificadoResponse>> ListarCertificadoById(int id);
}
