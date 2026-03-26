namespace Unievent.Domain.Entities;

public class Certificado : EntidadeBase
{
    public Certificado() { }
    public Certificado(DateTime dataCertificado, string texto, int idAluno, int idEvento)
    {
        DataCertifcado = dataCertificado;
        Texto = texto;
        IdAluno = idAluno;
        IdEvento = idEvento;
    }
    public required DateTime DataCertifcado { get; set; }
    public required string Texto { get; set; }
    public Aluno Aluno { get; set; }
    public int IdAluno { get; set; }
    public Evento Evento { get; set; }
    public int IdEvento { get; set; }
}
