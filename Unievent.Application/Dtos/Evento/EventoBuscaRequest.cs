using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Evento;

public record EventoBuscaRequest
{
    public int? InstituicaoId { get; init; }
    public string? InstituicaoCodigo { get; init; }
    public string? Curso { get; init; }
    public Categoria? Categoria { get; init; }
    public FiltroPublicoEvento? Publico { get; init; }
    public DateTime? Inicio { get; init; }
    public DateTime? Fim { get; init; }
    public string? Cidade { get; init; }
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
}
