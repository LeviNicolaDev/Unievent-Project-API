using Microsoft.AspNetCore.Http;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Evento;

public record EventoRequest
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public Categoria Categoria { get; set; }
    public DateTime DataEvento { get; set; }
    public int Capacidade { get; set; }
    public IList<IFormFile> Thumbnail { get; set; }
    public int ResponsavelEventoId { get; set; }
}
