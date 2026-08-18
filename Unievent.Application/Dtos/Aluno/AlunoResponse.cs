using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Aluno;

public record AlunoResponse
{
    public int Id { get; init; }
    public string Nome { get; init; }
    public string Email { get; init; }
    public bool IsAtivo { get; init; }
    public Role Role { get; init; }
    public string FotoPerfil { get; init; }
    public DateTime DataNascimento { get; init; }
    public TipoParticipante TipoParticipante { get; init; }
    public int? InstituicaoId { get; init; }
}
