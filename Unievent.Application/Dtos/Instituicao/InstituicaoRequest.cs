using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Dtos.Instituicao;

public record InstituicaoRequest
{
    public string? Nome { get; set; }
    public string? NomeAbreviado { get; set; }
    public string? Codigo { get; set; }
    public IFormFile FotoPerfil { get; set; }
    public string Cnpj { get; set; }
    public string Rua { get; set; }
    public string Cidade { get; set; }
    public string Bairro { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
    public string Numero { get; set; }
    public string? Telefone { get; set; }
    public string? Site { get; set; }
}
