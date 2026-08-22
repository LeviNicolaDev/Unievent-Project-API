using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Evento;

public record EventoResponse
{
    public int Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public string? Local { get; init; }
    public Categoria Categoria { get; init; }
    public DateTime DataEvento { get; init; }
    public int Capacidade { get; init; }
    public int? VagasDisponiveis { get; init; }
    public IList<string> Thumbnail { get; init; }
    public int IdResponsavelEvento { get; init; }
    public string Responsavel { get; init; }
    public int? InstituicaoId { get; init; }
    public string? InstituicaoNome { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public VisibilidadeEvento Visibilidade { get; init; }
    public PublicoPermitido PublicoPermitido { get; init; }
    public DateTime? InicioInscricoes { get; init; }
    public DateTime? FimInscricoes { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public int RaioCheckInMetros { get; init; }
}
