using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Dtos.Evento;

public record EventoRequest
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Categoria { get; set; }
    public string HoraEvento { get; set; }
    public DateTime DataEvento { get; set; }
    public int Capacidade { get; set; }
    public IList<IFormFile> Thumbnail { get; set; }

    public int ResponsavelEventoId { get; set; }
}
