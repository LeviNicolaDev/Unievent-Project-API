
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Auth;
using Unievent.Application.Interfaces.Repository;

namespace Unievent.Application.Services;

// Services/AuthService.cs
public class AuthService
{
    private readonly IUsuarioSecretariaRepository _repository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioSecretariaRepository repository,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<Result<string>> Login(UsuarioSecretariaLoginRequest request)
    {
        try
        {
            var usuario = await _repository.ListarUsuarioSecretariaByEmail(request.Email);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            {
                _logger.LogWarning("Tentativa de login inválida para {Email}", request.Email);
                return Result<string>.Failure("Email ou senha inválidos");
            }

            var token = _jwtTokenGenerator.GerarToken(usuario.Id, usuario.EmailUsuario, usuario.RoleUsuario);

            _logger.LogInformation("Login realizado com sucesso para {Email}", request.Email);
            return Result<string>.Success(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login para {Email}", request.Email);
            return Result<string>.Failure("Erro interno ao realizar login");
        }
    }
}