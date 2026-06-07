using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.UsuarioSecretaria;

public class UsuarioSecretariaRequest
{
    public string NomeUsuario { get; set; }
    public Role RoleUsuario { get; set; }
    public string Senha { get; set; }
    public string EmailUsuario { get; set; }
    public string Chave { get; set; }

}
