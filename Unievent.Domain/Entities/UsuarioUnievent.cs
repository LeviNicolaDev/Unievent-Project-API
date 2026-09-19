using Unievent.Domain.Enuns;

namespace Unievent.Domain.Entities;

public class UsuarioUnievent : EntidadeBase
{
    public required string NomeUsuario { get; set; }
    public required string EmailUsuario { get; set; }
    public required string Senha { get; set; }
    public string Chave { get; set; } = string.Empty;
    public Role RoleUsuario { get; set; } = Role.Admin;
    public int TentativasLogin { get; set; } = default;
    public bool IsAtivo { get; set; } = true;
    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
}
