namespace Unievent.Application.Dtos.Certificado;

public record CertificadoResponse
{
    public int Id { get; set; }
    public DateTime DataCertifcado { get; set; }
    public string Texto { get; set; }
    public int AlunoId { get; set; }
    public int EventoId { get; set; }
    public string NomeEvento { get; set; }
}
