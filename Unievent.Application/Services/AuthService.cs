
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Auth;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Services;

// Services/AuthService.cs
public class AuthService
{
    private readonly IUsuarioUnieventRepository _repositoryUsuarioUnievent;
    private readonly IUsuarioSecretariaRepository _repository;
    private readonly IAlunoRepository _repositoryAluno;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioUnieventRepository repositoryUsuarioUnievent,
        IUsuarioSecretariaRepository repository,
        IJwtTokenGenerator jwtTokenGenerator,
        IAlunoRepository repositoryAluno,
        ILogger<AuthService> logger)
    {
        _repositoryUsuarioUnievent = repositoryUsuarioUnievent;
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _repositoryAluno = repositoryAluno;
        _logger = logger;
    }

    public async Task<Result<string>> Login(UsuarioSecretariaLoginRequest request)
    {
        try
        {
            var usuarioUnievent = await _repositoryUsuarioUnievent.ListarUsuarioUnieventByEmail(request.Email);
            if (usuarioUnievent is not null)
            {
                if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuarioUnievent.Senha))
                {
                    _logger.LogWarning("Tentativa de login inválida para {Email}", request.Email);
                    return Result<string>.Failure("Email ou senha inválidos");
                }

                var tokenAdmin = _jwtTokenGenerator.GerarToken(usuarioUnievent.Id, usuarioUnievent.EmailUsuario, usuarioUnievent.RoleUsuario);

                _logger.LogInformation("Login do Admin UniEvent realizado com sucesso para {Email}", request.Email);
                return Result<string>.Success(tokenAdmin);
            }

            var usuario = await _repository.ListarUsuarioSecretariaByEmail(request.Email);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            {
                _logger.LogWarning("Tentativa de login inválida para {Email}", request.Email);
                return Result<string>.Failure("Email ou senha inválidos");
            }
            if (!usuario.EmailConfirmado)
            {
                _logger.LogWarning("Tentativa de login sem e-mail confirmado para {Email}", request.Email);
                return Result<string>.Failure("Confirme seu e-mail institucional antes de acessar.");
            }
            if (!usuario.IsAtivo || usuario.Status != StatusUsuarioSecretaria.Ativo)
            {
                _logger.LogWarning("Tentativa de login bloqueada para {Email} com status {Status}", request.Email, usuario.Status);
                return Result<string>.Failure(usuario.Status switch
                {
                    StatusUsuarioSecretaria.Pendente => "Cadastro aguardando aprovação.",
                    StatusUsuarioSecretaria.Recusado => "Cadastro não aprovado.",
                    StatusUsuarioSecretaria.Bloqueado => "Cadastro bloqueado.",
                    _ => "Cadastro inativo."
                });
            }

            var token = _jwtTokenGenerator.GerarToken(usuario.Id, usuario.EmailUsuario, usuario.RoleUsuario, usuario.InstituicaoId, usuario.Status);

            _logger.LogInformation("Login realizado com sucesso para {Email}", request.Email);
            return Result<string>.Success(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login para {Email}", request.Email);
            return Result<string>.Failure("Erro interno ao realizar login");
        }
    }
    public async Task<Result<string>> LoginAluno(LoginRequest request)
    {
        try
        {
            var usuario = await _repositoryAluno.ListarAlunoByEmail(request.Email);
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            {
                _logger.LogWarning("Tentativa de login inválida para {Email}", request.Email);
                return Result<string>.Failure("Email ou senha inválidos");
            }
            if (!usuario.EmailConfirmado)
            {
                _logger.LogWarning("Tentativa de login de aluno sem e-mail confirmado para {Email}", request.Email);
                return Result<string>.Failure("Confirme seu e-mail institucional antes de acessar.");
            }

            var token = _jwtTokenGenerator.GerarToken(usuario.Id, usuario.Email, usuario.Role, usuario.TipoParticipante, usuario.InstituicaoId);

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
