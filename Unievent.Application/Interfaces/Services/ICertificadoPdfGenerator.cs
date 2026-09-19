using Unievent.Application.Dtos.Certificado;

namespace Unievent.Application.Interfaces.Services;

public interface ICertificadoPdfGenerator
{
    CertificadoPdfArquivo Gerar(CertificadoPdfDados dados);
}
