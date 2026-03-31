namespace Unievent.Application.Dtos.Aluno;

public record AlunoResponse
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public bool IsAtivo { get; set; }
    public string FotoPerfil { get; set; }
    public DateTime DataNascimento { get; set; }
}
