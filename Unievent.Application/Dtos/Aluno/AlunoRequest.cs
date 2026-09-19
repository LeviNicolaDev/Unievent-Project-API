using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Dtos.Aluno;

public record AlunoRequest
{
    public string Nome { get; set; }
    public string Senha { get; set; }
    public string Email { get; set; }

    public IFormFile FotoPerfil { get; set; }
    public DateTime DataNascimento { get; set; }
    public Unievent.Domain.Enuns.TipoParticipante TipoParticipante { get; set; } = Unievent.Domain.Enuns.TipoParticipante.Externo;
    public int? InstituicaoId { get; set; }
}
