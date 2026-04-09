using Unievent.Application.Common;
using Unievent.Application.Dtos.Certificado;

namespace Unievent.Application.Interfaces.Services;

public interface ICertificadoService
{
    public Task<ResultData<CertificadoResponse>> CriarCertificado(CertificadoRequest request);
    public Task<ResultData<CertificadoResponse>> AtualizarCertificado(int id, CertificadoUpdate update);
    public Task<Result> DeletarCertificado(int id);
    public Task<ResultData<IEnumerable<CertificadoResponse>>> ListarCertificados();
    public Task<ResultData<CertificadoResponse>> ListarCertificadoById(int id);
}
