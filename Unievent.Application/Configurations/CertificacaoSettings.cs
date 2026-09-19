namespace Unievent.Application.Configurations;

public class CertificacaoSettings
{
    public int MaxTentativas { get; set; } = 5;
    public int RetryMinutos { get; set; } = 5;
    public int ReservaMinutos { get; set; } = 10;
}
