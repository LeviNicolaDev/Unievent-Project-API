namespace Unievent.Domain.Entities;

public class PreferenciaNotificacao : EntidadeBase
{
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public bool LembretesEventos { get; set; } = true;
    public bool AlertasCertificados { get; set; } = true;
    public bool Recomendacoes { get; set; } = true;
    public bool UsarLocalizacao { get; set; }
    public string? Categorias { get; set; }
    public double? LatitudeAproximada { get; set; }
    public double? LongitudeAproximada { get; set; }
    public int RaioKm { get; set; } = 30;
    public DateTime AtualizadoEmUtc { get; set; } = DateTime.UtcNow;
}
