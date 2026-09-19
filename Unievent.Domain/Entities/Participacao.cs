using Unievent.Domain.Enuns;

namespace Unievent.Domain.Entities;

public class Participacao
{
    public Participacao(int alunoId, int eventoId)
    {
        AlunoId = alunoId;
        EventoId = eventoId;
        PresencaConfirmada = false;
        CertificadoEmitido = false;
        StatusInscricao = StatusInscricao.Ativa;
        CodigoIngresso = Guid.NewGuid().ToString("N");

    }
    public int Id { get; set; }

    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;

    public int EventoId { get; set; }
    public Evento Evento { get; set; } = null!;

    public bool PresencaConfirmada { get; private set; }

    public StatusInscricao StatusInscricao { get; private set; } = StatusInscricao.Ativa;

    public DateTime? DataCancelamento { get; private set; }

    public DateTime? DataConfirmacao { get; private set; }

    public bool CertificadoEmitido { get; private set; }

    public string? CodigoValidacao { get; private set; }
    public bool CertificadoEnviadoPorEmail { get; private set; }
    public DateTime? DataEnvioCertificadoEmail { get; private set; }
    public string? ErroEnvioCertificadoEmail { get; private set; }
    public byte[]? CertificadoPdf { get; private set; }
    public string? NomeArquivoCertificado { get; private set; }
    public DateTime? DataGeracaoCertificado { get; private set; }
    public string? DestinatarioCertificadoEmail { get; private set; }
    public StatusEnvioCertificado StatusEnvioCertificado { get; private set; }
    public Guid? ProcessamentoCertificadoId { get; private set; }
    public DateTime? ProcessamentoCertificadoAteUtc { get; private set; }
    public DateTime? ProximaTentativaCertificadoUtc { get; private set; }
    public int TentativasEnvioCertificado { get; private set; }
    public string CodigoIngresso { get; private set; } = Guid.NewGuid().ToString("N");
    public double? DistanciaCheckInMetros { get; private set; }
    public double? PrecisaoLocalizacaoMetros { get; private set; }
    public int? OperadorCheckInId { get; private set; }

    public void ConfirmarPresenca()
    {
        if (StatusInscricao == StatusInscricao.Cancelada)
            throw new InvalidOperationException("Inscrição cancelada não pode realizar check-in.");
        if (PresencaConfirmada) return;
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
    }

    public void ConfirmarPresencaPorCodigo(int operadorId)
    {
        if (StatusInscricao == StatusInscricao.Cancelada)
            throw new InvalidOperationException("Inscrição cancelada não pode realizar check-in.");
        if (PresencaConfirmada) return;
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
        OperadorCheckInId = operadorId;
    }

    public void ConfirmarPresenca(double distanciaMetros, double precisaoMetros, int operadorId)
    {
        if (StatusInscricao == StatusInscricao.Cancelada)
            throw new InvalidOperationException("Inscrição cancelada não pode realizar check-in.");
        if (PresencaConfirmada) return;
        PresencaConfirmada = true;
        DataConfirmacao = DateTime.UtcNow;
        DistanciaCheckInMetros = distanciaMetros;
        PrecisaoLocalizacaoMetros = precisaoMetros;
        OperadorCheckInId = operadorId;
    }

    public void CancelarInscricao()
    {
        if (PresencaConfirmada)
            throw new InvalidOperationException("Não é possível cancelar uma inscrição com check-in realizado.");

        if (StatusInscricao == StatusInscricao.Cancelada) return;
        StatusInscricao = StatusInscricao.Cancelada;
        DataCancelamento = DateTime.UtcNow;
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
        StatusEnvioCertificado = StatusEnvioCertificado.Enviado;
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
            : erro[..Math.Min(erro.Length, 500)];
    }
}
