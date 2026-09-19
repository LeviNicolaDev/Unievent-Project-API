using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.UsuarioSecretaria;

public class UsuarioSecretariaUpdate
{
    public string? NomeUsuario { get; set; }
    public string? EmailUsuario { get; set; }
    public Role? Role { get; set; }
    public string? Senha { get; set; }
    public int? InstituicaoId { get; set; }
    public StatusUsuarioSecretaria? Status { get; set; }


}
