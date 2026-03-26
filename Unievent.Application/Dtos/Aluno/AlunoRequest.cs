namespace Unievent.Application.Dtos.Aluno;

public record AlunoRequest
{
    public string Nome { get; set; }
    public string Senha { get; set; }
    public string Email { get; set; }

    public IFormFile FotoPerfil { get; set; }
    public DateTime DataNascimento { get; set; }
}
