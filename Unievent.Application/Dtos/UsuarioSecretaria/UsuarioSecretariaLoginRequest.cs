namespace Unievent.Application.Dtos.UsuarioSecretaria;

public record UsuarioSecretariaLoginRequest
{
    public string Email { get; set; }
    public string Senha { get; set; }
}
