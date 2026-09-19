namespace Unievent.Application.Dtos.Dashboard;

public record InstituicaoDashboardResponse
{
    public int InstituicaoId { get; init; }
    public string InstituicaoNome { get; init; } = string.Empty;
    public int TotalEventos { get; init; }
    public int EventosFuturos { get; init; }
    public int EventosRealizados { get; init; }
    public int TotalResponsaveis { get; init; }
    public int TotalInscricoes { get; init; }
    public int TotalPresencas { get; init; }
    public int TotalCertificadosEmitidos { get; init; }
    public double TaxaComparecimento { get; init; }
    public IReadOnlyCollection<DashboardEventoResumoResponse> EventosRecentes { get; init; } =
        Array.Empty<DashboardEventoResumoResponse>();
    public IReadOnlyCollection<DashboardEventoResumoResponse> ProximosEventos { get; init; } =
        Array.Empty<DashboardEventoResumoResponse>();
    public IReadOnlyCollection<DashboardEventoMetricasResponse> MetricasPorEvento { get; init; } =
        Array.Empty<DashboardEventoMetricasResponse>();
}

public record DashboardEventoResumoResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public DateTime DataEvento { get; init; }
    public int Inscricoes { get; init; }
    public int Presencas { get; init; }
    public int CertificadosEmitidos { get; init; }
}

public record DashboardEventoMetricasResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public DateTime DataEvento { get; init; }
    public int Capacidade { get; init; }
    public int Inscricoes { get; init; }
    public int Presentes { get; init; }
    public int Ausentes { get; init; }
    public int CertificadosEmitidos { get; init; }
    public double TaxaComparecimento { get; init; }
}
