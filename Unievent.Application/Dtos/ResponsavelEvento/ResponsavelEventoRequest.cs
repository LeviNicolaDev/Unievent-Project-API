using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Dtos.ResponsavelEvento;

public class ResponsavelEventoRequest
{
    public string Nome { get; set; }
    public IFormFile FotoPerfil { get; set; }
    public int? InstituicaoId { get; set; }
}
