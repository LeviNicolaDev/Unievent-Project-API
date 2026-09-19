namespace Unievent.Application.Dtos.Auth;

public record PublicoGeralCadastroRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public string ConfirmacaoSenha { get; init; } = string.Empty;
}
