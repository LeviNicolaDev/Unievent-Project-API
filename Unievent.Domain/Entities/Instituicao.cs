namespace Unievent.Domain.Entities;

public class Instituicao : EntidadeBase
{
    public Instituicao()
    {

    }
    public Instituicao(int id, string fotoPerfil, string cnpj)
    {
        Id = id;
        FotoPerfil = fotoPerfil;
        Cnpj = cnpj;
    }
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? NomeAbreviado { get; set; }
    public string? Codigo { get; set; }
    public required string FotoPerfil { get; set; }
    public required string Cnpj { get; set; }
    public required string Rua { get; set; }
    public required string Cidade { get; set; }
    public required string Bairro { get; set; }
    public required string Estado { get; set; }
    public required string Cep { get; set; }
    public required string Numero { get; set; }
    public string? Telefone { get; set; }
    public string? Site { get; set; }
    public bool IsAtivo { get; set; } = true;
    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEmUtc { get; set; } = DateTime.UtcNow;
}
