
using FluentValidation;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Auth;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Auth;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
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
    private readonly IValidator<PublicoGeralCadastroRequest> _publicoGeralValidator;

    public AuthService(
        IUsuarioUnieventRepository repositoryUsuarioUnievent,
        IUsuarioSecretariaRepository repository,
        IJwtTokenGenerator jwtTokenGenerator,
        IAlunoRepository repositoryAluno,
        ILogger<AuthService> logger,
        IValidator<PublicoGeralCadastroRequest> publicoGeralValidator)
    {
        _repositoryUsuarioUnievent = repositoryUsuarioUnievent;
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _repositoryAluno = repositoryAluno;
        _logger = logger;
        _publicoGeralValidator = publicoGeralValidator;
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

    public async Task<Result<AlunoResponse>> CadastrarPublicoGeral(PublicoGeralCadastroRequest request)
    {
        try
        {
            var validationResult = await _publicoGeralValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result<AlunoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var emailNormalizado = request.Email.Trim();
            if (await _repositoryAluno.ListarAlunoByEmail(emailNormalizado) is not null)
            {
                return Result<AlunoResponse>.Failure("Email já cadastrado para outro participante");
            }

            if (await _repository.ListarUsuarioSecretariaByEmail(emailNormalizado) is not null ||
                await _repositoryUsuarioUnievent.ListarUsuarioUnieventByEmail(emailNormalizado) is not null)
            {
                return Result<AlunoResponse>.Failure("Email já cadastrado para outro usuário");
            }

            var aluno = new Aluno
            {
                Nome = request.Nome.Trim(),
                Email = emailNormalizado,
                EmailConfirmado = true,
                ChaveConfirmacaoEmail = null,
                FotoPerfil = string.Empty,
                DataNascimento = DateTime.UtcNow.Date.AddYears(-18),
                IsAtivo = true,
                Role = Role.Aluno,
                Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                TipoParticipante = TipoParticipante.Externo,
                InstituicaoId = null
            };

            await _repositoryAluno.CriarAluno(aluno);
            await _repositoryAluno.SaveChangesAsync();

            return Result<AlunoResponse>.Success(new AlunoResponse
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                EmailConfirmado = aluno.EmailConfirmado,
                FotoPerfil = aluno.FotoPerfil,
                IsAtivo = aluno.IsAtivo,
                Role = aluno.Role,
                DataNascimento = aluno.DataNascimento,
                TipoParticipante = aluno.TipoParticipante,
                InstituicaoId = aluno.InstituicaoId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar público geral para {Email}", request.Email);
            return Result<AlunoResponse>.Failure("Erro interno ao cadastrar público geral");
        }
    }

    public async Task<Result<string>> LoginPublicoGeral(LoginRequest request)
    {
        try
        {
            var usuario = await _repositoryAluno.ListarAlunoByEmail(request.Email);
            if (usuario is null || usuario.TipoParticipante != TipoParticipante.Externo ||
                !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
            {
                _logger.LogWarning("Tentativa de login público inválida para {Email}", request.Email);
                return Result<string>.Failure("Email ou senha inválidos");
            }

            if (!usuario.EmailConfirmado)
            {
                return Result<string>.Failure("Confirme seu e-mail antes de acessar.");
            }

            var token = _jwtTokenGenerator.GerarToken(usuario.Id, usuario.Email, usuario.Role, usuario.TipoParticipante, usuario.InstituicaoId);
            return Result<string>.Success(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao realizar login público para {Email}", request.Email);
            return Result<string>.Failure("Erro interno ao realizar login");
        }
    }

}
