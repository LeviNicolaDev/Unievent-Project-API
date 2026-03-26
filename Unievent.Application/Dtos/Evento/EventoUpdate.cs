namespace Unievent.Application.Dtos.Evento;

public record EventoUpdate
{

    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public string? Categoria { get; set; }
    public string? HoraEvento { get; set; }
    public DateTime? DataEvento { get; set; }
    public int? Capacidade { get; set; }
    public IList<string>? Thumbnail { get; set; }
    public string? ThumbnailOpcional { get; set; }
    public int? IdResponsavelEvento { get; set; }
}
