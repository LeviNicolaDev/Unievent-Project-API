namespace Unievent.Application.Dtos.UsuarioUnievent;

public class UsuarioUnieventResponse
{
    public int Id { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public string RoleUsuario { get; set; } = string.Empty;
    public bool IsAtivo { get; set; }
}
