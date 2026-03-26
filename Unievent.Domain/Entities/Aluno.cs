namespace Unievent.Domain.Entities;

public class Aluno : EntidadeBase
{
    public Aluno()
    {

    }
    public Aluno(string nome, string senha, string email, string fotoPerfil, bool isAtivo, DateTime dataNascimento)
    {

        Nome = nome;
        Senha = senha;
        Email = email;
        FotoPerfil = fotoPerfil;
        IsAtivo = isAtivo;
        DataNascimento = dataNascimento;

    }
    public required string Nome { get; set; }
    public required string Senha { get; set; }
    public required string Email { get; set; }
    public required string FotoPerfil { get; set; }
    public bool IsAtivo { get; set; }
    public required DateTime DataNascimento { get; set; }
}
