namespace Unievent.Domain.Entities;

public class Participacao
{
    public Participacao(int alunoId, int eventoId)
    {
        AlunoId = alunoId;
        EventoId = eventoId;
        PresencaConfirmada = false;
        CertificadoEmitido = false;

    }
    public int Id { get; set; }

    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; }

    public int EventoId { get; set; }
    public Evento Evento { get; set; }

    public bool PresencaConfirmada { get; private set; }

    public DateTime? DataConfirmacao { get; private set; }

    public bool CertificadoEmitido { get; private set; }

    public string? CodigoValidacao { get; private set; }

    public void ConfirmarPresenca()
    {
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
    }

    public void EmitirCertificado(string codigo)
    {
        if (!PresencaConfirmada)
            throw new Exception("Aluno sem presença confirmada.");

        CertificadoEmitido = true;
        CodigoValidacao = codigo;
    }
}