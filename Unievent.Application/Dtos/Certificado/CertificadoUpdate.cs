namespace Unievent.Application.Dtos.Certificado;

public record CertificadoUpdate
{
    public DateTime? DataCertifcado { get; set; }
    public string? Texto { get; set; }
    public int? IdAluno { get; set; }
    public int? IdEvento { get; set; }
}
