namespace Unievent.Application.Dtos.Participacao;

public record ParticipacaoResponse
{
    public int Id { get; init; }
    public string NomeAluno { get; init; }
    public int AlunoId { get; init; }
    public DateTime? DataConfirmacao { get; init; }
    public int EventoId { get; init; }
    public bool PresencaGarantida { get; init; }
    public bool CertificadoEmitido { get; init; }
    public bool CertificadoEnviadoPorEmail { get; init; }
    public string? ErroEnvioCertificadoEmail { get; init; }
}
