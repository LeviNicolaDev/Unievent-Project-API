using Microsoft.AspNetCore.Http;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Evento;

public record EventoRequest
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string? Local { get; set; }
    public Categoria Categoria { get; set; }
    public DateTime DataEvento { get; set; }
    public int Capacidade { get; set; }
    public IList<IFormFile> Thumbnail { get; set; }
    public int ResponsavelEventoId { get; set; }
    public int? InstituicaoId { get; set; }
    public VisibilidadeEvento Visibilidade { get; set; } = VisibilidadeEvento.Publico;
    public PublicoPermitido PublicoPermitido { get; set; } = PublicoPermitido.PublicoGeral;
    public DateTime? InicioInscricoes { get; set; }
    public DateTime? FimInscricoes { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int RaioCheckInMetros { get; set; } = 150;
}
