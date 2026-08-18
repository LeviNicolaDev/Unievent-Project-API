namespace Unievent.Domain.Entities;

public class Participacao
{
    public Participacao(int alunoId, int eventoId)
    {
        AlunoId = alunoId;
        EventoId = eventoId;
        PresencaConfirmada = false;
        CertificadoEmitido = false;
        CodigoIngresso = Guid.NewGuid().ToString("N");

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
    public string CodigoIngresso { get; private set; } = Guid.NewGuid().ToString("N");
    public double? DistanciaCheckInMetros { get; private set; }
    public double? PrecisaoLocalizacaoMetros { get; private set; }
    public int? OperadorCheckInId { get; private set; }

    public void ConfirmarPresenca()
    {
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
    }

    public void ConfirmarPresenca(double distanciaMetros, double precisaoMetros, int operadorId)
    {
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
        DistanciaCheckInMetros = distanciaMetros;
        PrecisaoLocalizacaoMetros = precisaoMetros;
        OperadorCheckInId = operadorId;
    }

    public void EmitirCertificado(string codigo)
    {
        if (!PresencaConfirmada)
            throw new Exception("Aluno sem presença confirmada.");

        CertificadoEmitido = true;
        CodigoValidacao = codigo;
    }
}
