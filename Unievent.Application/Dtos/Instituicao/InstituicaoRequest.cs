using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Dtos.Instituicao;

public record InstituicaoRequest
{
    public string EmailLogin { get; set; }
    public string SenhaLogin { get; set; }
    public IFormFile FotoPerfil { get; set; }
    public string Cnpj { get; set; }
    public int IdEndereco { get; set; }
}
