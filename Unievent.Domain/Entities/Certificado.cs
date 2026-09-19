namespace Unievent.Domain.Entities;

public class Certificado : EntidadeBase
{
    public Certificado() { }
    public Certificado(DateTime dataCertificado, string texto, int eventoId)
    {
        DataCertifcado = dataCertificado;
        Texto = texto;
        EventoId = eventoId;
    }
    public required DateTime DataCertifcado { get; set; }
    public required string Texto { get; set; }
    public required int EventoId { get; set; }
    public Evento Evento { get; set; }

}
