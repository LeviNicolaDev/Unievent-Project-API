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
    public bool CertificadoEnviadoPorEmail { get; private set; }
    public DateTime? DataEnvioCertificadoEmail { get; private set; }
    public string? ErroEnvioCertificadoEmail { get; private set; }
    public string CodigoIngresso { get; private set; } = Guid.NewGuid().ToString("N");
    public double? DistanciaCheckInMetros { get; private set; }
    public double? PrecisaoLocalizacaoMetros { get; private set; }
    public int? OperadorCheckInId { get; private set; }

    public void ConfirmarPresenca()
    {
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
    }

    public void ConfirmarPresencaPorCodigo(int operadorId)
    {
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
        OperadorCheckInId = operadorId;
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

        if (CertificadoEmitido && !string.IsNullOrWhiteSpace(CodigoValidacao))
            return;

        CertificadoEmitido = true;
        CodigoValidacao = string.IsNullOrWhiteSpace(CodigoValidacao) ? codigo : CodigoValidacao;
    }

    public void RegistrarEnvioCertificadoEmail()
    {
        if (!CertificadoEmitido)
            throw new Exception("Certificado ainda não foi emitido.");

        CertificadoEnviadoPorEmail = true;
        DataEnvioCertificadoEmail = DateTime.UtcNow;
        ErroEnvioCertificadoEmail = null;
    }

    public void RegistrarFalhaEnvioCertificadoEmail(string erro)
    {
        if (!CertificadoEmitido)
            throw new Exception("Certificado ainda não foi emitido.");

        CertificadoEnviadoPorEmail = false;
        ErroEnvioCertificadoEmail = string.IsNullOrWhiteSpace(erro)
            ? "Falha ao enviar e-mail do certificado"
            : erro;
    }
}
