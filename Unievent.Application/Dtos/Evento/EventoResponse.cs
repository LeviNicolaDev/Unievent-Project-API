namespace Unievent.Application.Dtos.Evento;

public record EventoResponse
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Categoria { get; set; }
    public string HoraEvento { get; set; }
    public DateTime DataEvento { get; set; }
    public int Capacidade { get; set; }
    public IList<string> Thumbnail { get; set; }
    public int IdResponsavelEvento { get; set; }
}
