using Microsoft.AspNetCore.Http;

namespace Unievent.Application.Dtos.ResponsavelEvento;

public class ResponsavelEventoUpdate
{
    public string? Nome { get; set; }
    public IFormFile? FotoPerfil { get; set; }
    public int? InstituicaoId { get; set; }
}
