namespace Unievent.Application.Dtos.UsuarioSecretaria;

public class UsuarioSecretariaResponse
{
    public int Id { get; set; }
    public string NomeUsuario { get; set; }
    public string RoleUsuario { get; set; }
    public string EmailUsuario { get; set; }

    public string Chave { get; set; }
    public bool IsAtivo { get; set; }
}
