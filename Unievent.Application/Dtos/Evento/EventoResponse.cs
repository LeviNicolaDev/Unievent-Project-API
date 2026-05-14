using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Evento;

public record EventoResponse
{
    public int Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public Categoria Categoria { get; init; }
    public DateTime DataEvento { get; init; }
    public int Capacidade { get; init; }
    public IList<string> Thumbnail { get; init; }
    public int IdResponsavelEvento { get; init; }
}
