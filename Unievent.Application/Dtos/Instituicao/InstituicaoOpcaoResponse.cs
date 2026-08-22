namespace Unievent.Application.Dtos.Instituicao;

public record InstituicaoOpcaoResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Sigla { get; init; }
}
