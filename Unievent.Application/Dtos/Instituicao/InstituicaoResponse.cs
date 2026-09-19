namespace Unievent.Application.Dtos.Instituicao;

public class InstituicaoResponse
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? NomeAbreviado { get; set; }
    public string? Codigo { get; set; }
    public string FotoPerfil { get; set; }
    public string Cnpj { get; set; }
    public string? Rua { get; set; }
    public string? Cidade { get; set; }
    public string? Bairro { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public string? Numero { get; set; }
    public string? Telefone { get; set; }
    public string? Site { get; set; }
    public bool IsAtivo { get; set; }
}
