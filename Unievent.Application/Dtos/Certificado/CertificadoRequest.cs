namespace Unievent.Application.Dtos.Certificado;

public record CertificadoRequest
{
    public DateTime DataCertifcado { get; set; }
    public string Texto { get; set; }
    public int IdAluno { get; set; }
    public int IdEvento { get; set; }
}
