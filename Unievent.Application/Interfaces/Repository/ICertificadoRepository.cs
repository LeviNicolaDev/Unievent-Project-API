namespace Unievent.Application.Interfaces.Repository;

public interface ICertificadoRepository
{
    public Task<Certificado> CriarCertificado(Certificado certificado);
    public Task<Certificado> AtualizarCertificado(Certificado certificado);
    public Task<IList<Certificado>> ListarCertificados();
    public Task SaveChangesAsync();
    public Task<Certificado> ListarCertificadoById(int id);
    public Task<bool> DeletarCertificado(Certificado certificado);
}
