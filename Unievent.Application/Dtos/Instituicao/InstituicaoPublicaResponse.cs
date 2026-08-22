namespace Unievent.Application.Dtos.Instituicao;

public record InstituicaoPublicaResponse
{
    public int? Id { get; init; }
    public required string Codigo { get; init; }
    public required string Nome { get; init; }
    public string? NomeAbreviado { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public bool TemEventos { get; init; }
    public int TotalEventos { get; init; }
}
