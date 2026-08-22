namespace Unievent.Application.Dtos.Dashboard;

public record AdminDashboardResponse
{
    public int TotalInstituicoes { get; init; }
    public int InstituicoesAtivas { get; init; }
    public int TotalSecretarias { get; init; }
    public int SecretariasPendentes { get; init; }
    public int TotalAlunos { get; init; }
    public int TotalEventos { get; init; }
    public int TotalInscricoes { get; init; }
    public int TotalPresencas { get; init; }
    public int TotalCertificadosEmitidos { get; init; }
    public double TaxaComparecimento { get; init; }
    public IReadOnlyCollection<AdminInstituicaoMetricasResponse> Instituicoes { get; init; } =
        Array.Empty<AdminInstituicaoMetricasResponse>();
}

public record AdminInstituicaoMetricasResponse
{
    public int InstituicaoId { get; init; }
    public string InstituicaoNome { get; init; } = string.Empty;
    public bool Ativa { get; init; }
    public int Eventos { get; init; }
    public int Inscricoes { get; init; }
    public int Presencas { get; init; }
    public int CertificadosEmitidos { get; init; }
    public double TaxaComparecimento { get; init; }
}
