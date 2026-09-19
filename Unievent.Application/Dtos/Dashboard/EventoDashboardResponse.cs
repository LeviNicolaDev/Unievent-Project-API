using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Dashboard;

public record EventoDashboardResponse
{
    public int EventoId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public DateTime DataEvento { get; init; }
    public string? Local { get; init; }
    public int? InstituicaoId { get; init; }
    public string InstituicaoNome { get; init; } = string.Empty;
    public int ResponsavelEventoId { get; init; }
    public string ResponsavelEventoNome { get; init; } = string.Empty;
    public PublicoPermitido PublicoPermitido { get; init; }
    public int CapacidadeTotal { get; init; }
    public int TotalInscricoes { get; init; }
    public int VagasRestantes { get; init; }
    public int CheckInsRealizados { get; init; }
    public int AusentesSemCheckIn { get; init; }
    public double PercentualOcupacao { get; init; }
    public double PercentualPresenca { get; init; }
    public string StatusEvento { get; init; } = string.Empty;
    public bool PossuiCertificado { get; init; }
    public int CertificadosEmitidos { get; init; }
    public int InscricoesPublicoGeral { get; init; }
    public int InscricoesAlunosFatec { get; init; }
}
