namespace Unievent.Application.Dtos.Aluno;

public record AlunoUpdate
{
    public string? Nome { get; set; }
    public string? Senha { get; set; }
    public IFormFile? FotoPerfil { get; set; }
    public DateTime? DataNascimento { get; set; }
}
