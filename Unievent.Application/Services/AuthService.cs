using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
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

    public async Task<ResultData<string>> Login(LoginRequest request)
    {
        try
        {
            var usuario = await _repository.ListarUsuarioSecretariaByEmail(request.Email);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.Senha))
            {
                _logger.LogWarning("Tentativa de login inválida para {Email}", request.Email);
                return ResultData<string>.Failure("Email ou senha inválidos");
            }

            var token = _jwtTokenGenerator.GerarToken(usuario.Id, usuario.EmailUsuario, usuario.RoleUsuario);

            _logger.LogInformation("Login realizado com sucesso para {Email}", request.Email);
            return new ResultData<string>(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login para {Email}", request.Email);
            return ResultData<string>.Failure("Erro interno ao realizar login");
        }
    }
}