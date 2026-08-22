using FluentValidation;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Services
{
    public class UsuarioSecretariaService : IUsuarioSecretariaService
    {
        private readonly IUsuarioSecretariaRepository _repository;
        private readonly ILogger<UsuarioSecretariaService> _logger;
        private readonly IValidator<UsuarioSecretariaRequest> _requestValidator;
        private readonly IValidator<UsuarioSecretariaUpdate> _updateValidator;
        private readonly IEmailService? _emailService;
        public UsuarioSecretariaService(IUsuarioSecretariaRepository repository, ILogger<UsuarioSecretariaService> logger,
        IValidator<UsuarioSecretariaRequest> requestValidator, IValidator<UsuarioSecretariaUpdate> updateValidator, IEmailService? emailService = null)
        {
            _repository = repository;
            _logger = logger;
            _requestValidator = requestValidator;
            _updateValidator = updateValidator;
            _emailService = emailService;
        }


        async Task<Result<UsuarioSecretariaResponse>> IUsuarioSecretariaService.AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update)
        {
            try
            {
                _logger.LogInformation("Iniciando atualização do usuário da secretaria com ID {UsuarioSecretariaId}", id);
                var validator = await _updateValidator.ValidateAsync(update);
                if (!validator.IsValid)
                    return Result<UsuarioSecretariaResponse>.Failure(validator.Errors.Select(e => e.ErrorMessage).ToList());
                var emailExistente = await _repository.ListarUsuarioSecretariaByEmail(update.EmailUsuario);
                var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com ID {UsuarioSecretariaId} não encontrado para atualização", id);
                    return Result<UsuarioSecretariaResponse>.Failure("UsuarioSecretaria não encontrado");
                }
                if (!string.IsNullOrWhiteSpace(update.EmailUsuario) && emailExistente == null)
                {
                    usuarioSecretaria.EmailUsuario = update.EmailUsuario;
                }
                else if (!string.IsNullOrWhiteSpace(update.EmailUsuario) && emailExistente != null)
                {
                    _logger.LogWarning("Email {Email} já cadastrado para outro usuário da secretaria", update.EmailUsuario);
                    return Result<UsuarioSecretariaResponse>.Failure("Email já cadastrado para outro usuário da secretaria");
                }
                if (!string.IsNullOrWhiteSpace(update.NomeUsuario))
                {
                    usuarioSecretaria.NomeUsuario = update.NomeUsuario;
                }
                if (!string.IsNullOrWhiteSpace(update.Senha))
                {
                    var senhaHash = BCrypt.Net.BCrypt.HashPassword(update.Senha);
                    usuarioSecretaria.Senha = senhaHash;
                }
                if (update.Role.HasValue && update.Role != usuarioSecretaria.RoleUsuario)
                {

                    usuarioSecretaria.RoleUsuario = update.Role.Value;
                }
                else if (update.Role.HasValue)
                {
                    _logger.LogWarning("Cargo {Role} inválido para o usuário da secretaria com ID {UsuarioSecretariaId}. Digite 'Secretaria' ou 'Admin'", update.Role, id);
                    return Result<UsuarioSecretariaResponse>.Failure("Cargo inválido para o usuário da secretaria ou usuario secretaria ja tem esse cargo");
                }
                if (update.InstituicaoId.HasValue)
                {
                    usuarioSecretaria.InstituicaoId = update.InstituicaoId;
                }
                if (update.Status.HasValue)
                {
                    usuarioSecretaria.Status = update.Status.Value;
                }

                await _repository.AtualizarUsuarioSecretaria(usuarioSecretaria);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Usuário da secretaria com ID {UsuarioSecretariaId} atualizado com sucesso", id);
                return Result<UsuarioSecretariaResponse>.Success(MapearResponse(usuarioSecretaria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário da secretaria com ID {UsuarioSecretariaId}", id);
                return Result<UsuarioSecretariaResponse>.Failure($"Erro ao atualizar usuário da secretaria");
            }
        }

        async Task<Result<UsuarioSecretariaResponse>> IUsuarioSecretariaService.CriarUsuarioSecretaria(UsuarioSecretariaRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando criação do usuário da secretaria com email {EmailUsuario}", request.EmailUsuario);
                var validator = await _requestValidator.ValidateAsync(request);
                if (!validator.IsValid)
                    return Result<UsuarioSecretariaResponse>.Failure(validator.Errors.Select(e => e.ErrorMessage).ToList());

                var role = Enum.TryParse(request.RoleUsuario.ToString(), out Role result) ? result : Role.Secretaria;
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
                var emailExistente = await _repository.ListarUsuarioSecretariaByEmail(request.EmailUsuario);
                if (emailExistente != null)
                {
                    _logger.LogWarning("Email {EmailUsuario} já cadastrado para outro usuário da secretaria", request.EmailUsuario);
                    return Result<UsuarioSecretariaResponse>.Failure("Email já cadastrado para outro usuário da secretaria");
                }
                var chaveConfirmacaoEmail = string.IsNullOrWhiteSpace(request.Chave)
                    ? Guid.NewGuid().ToString("N")
                    : request.Chave;
                var usuarioSecretaria = new UsuarioSecretaria
                {
                    NomeUsuario = request.NomeUsuario,
                    Chave = chaveConfirmacaoEmail,
                    Senha = senhaHash,
                    EmailUsuario = request.EmailUsuario,
                    EmailConfirmado = false,
                    RoleUsuario = role,
                    IsAtivo = true,
                    Status = request.Status ?? StatusUsuarioSecretaria.Ativo,
                    InstituicaoId = request.InstituicaoId
                };
                var emailConfirmacao = await EnviarEmailConfirmacaoConta(
                    usuarioSecretaria.EmailUsuario,
                    usuarioSecretaria.NomeUsuario,
                    chaveConfirmacaoEmail);
                if (emailConfirmacao.IsFailure)
                {
                    return Result<UsuarioSecretariaResponse>.Failure(emailConfirmacao.Errors);
                }
                await _repository.CriarUsuarioSecretaria(usuarioSecretaria);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Usuário da secretaria com email {EmailUsuario} criado com sucesso", request.EmailUsuario);
                return Result<UsuarioSecretariaResponse>.Success(MapearResponse(usuarioSecretaria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário da secretaria com email {EmailUsuario}", request.EmailUsuario);
                return Result<UsuarioSecretariaResponse>.Failure("Erro ao criar usuário da secretaria");
            }
        }

        async Task<Result<bool>> IUsuarioSecretariaService.DeletarUsuarioSecretaria(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando deleção do usuário da secretaria com ID {UsuarioSecretariaId}", id);
                var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com ID {UsuarioSecretariaId} não encontrado para deleção", id);
                    return Result<bool>.Failure("UsuarioSecretaria não encontrado");
                }
                usuarioSecretaria.IsAtivo = false;
                usuarioSecretaria.Status = StatusUsuarioSecretaria.Bloqueado;
                await _repository.AtualizarUsuarioSecretaria(usuarioSecretaria);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Usuário da secretaria com ID {UsuarioSecretariaId} deletado com sucesso", id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário da secretaria com ID {UsuarioSecretariaId}", id);
                return Result<bool>.Failure("Erro ao deletar usuário da secretaria");
            }
        }

        async Task<Result<IEnumerable<UsuarioSecretariaResponse>>> IUsuarioSecretariaService.ListarUsuarioSecretaria()
        {
            try
            {
                _logger.LogInformation("Iniciando listagem de usuários da secretaria");
                var usuarioSecretarias = await _repository.ListarUsuarioSecretarias();
                _logger.LogInformation("Usuários da secretaria listados com sucesso {UsuarioSecretariasCount}", usuarioSecretarias.Count());
                return Result<IEnumerable<UsuarioSecretariaResponse>>.Success(usuarioSecretarias.Select(MapearResponse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários da secretaria");
                return Result<IEnumerable<UsuarioSecretariaResponse>>.Failure("Erro ao listar usuários da secretaria");
            }
        }

        async Task<Result<UsuarioSecretariaResponse>> IUsuarioSecretariaService.ListarUsuarioSecretariaById(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando busca do usuário da secretaria com ID {UsuarioSecretariaId}", id);
                var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com ID {UsuarioSecretariaId} não encontrado", id);
                    return Result<UsuarioSecretariaResponse>.Failure("UsuarioSecretaria não encontrado");
                }
                _logger.LogInformation("Usuário da secretaria com ID {UsuarioSecretariaId} encontrado com sucesso", id);
                return Result<UsuarioSecretariaResponse>.Success(MapearResponse(usuarioSecretaria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário da secretaria com ID {UsuarioSecretariaId}", id);
                return Result<UsuarioSecretariaResponse>.Failure("Erro ao buscar usuário da secretaria");
            }
        }

        async Task<Result<UsuarioSecretariaLoginResponse>> IUsuarioSecretariaService.Login(UsuarioSecretariaLoginRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando login do usuário da secretaria com email {EmailUsuario}", request.Email);
                var usuarioSecretaria = _repository.ListarUsuarioSecretariaByEmail(request.Email).Result;
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com email {EmailUsuario} não encontrado para login", request.Email);
                    return Result<UsuarioSecretariaLoginResponse>.Failure("UsuarioSecretaria não encontrado");
                }
                if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuarioSecretaria.Senha))
                {
                    _logger.LogWarning("Tentativa de login com email {EmailUsuario} falhou: senha incorreta", request.Email);
                    return Result<UsuarioSecretariaLoginResponse>.Failure("Senha incorreta");
                }
                _logger.LogInformation("Usuário da secretaria com email {EmailUsuario} logado com sucesso", request.Email);
                return await Task.FromResult(Result<UsuarioSecretariaLoginResponse>.Success(new UsuarioSecretariaLoginResponse
                {

                    Token = "sjikjd"

                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login do usuário da secretaria com email {EmailUsuario}", request.Email);
                return Result<UsuarioSecretariaLoginResponse>.Failure("Erro ao realizar login do usuário da secretaria");
            }
        }

        async Task<Result<UsuarioSecretariaResponse>> IUsuarioSecretariaService.AlterarStatusUsuarioSecretaria(int id, StatusUsuarioSecretaria status)
        {
            try
            {
                var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com ID {UsuarioSecretariaId} não encontrado para alteração de status", id);
                    return Result<UsuarioSecretariaResponse>.Failure("UsuarioSecretaria não encontrado");
                }

                usuarioSecretaria.Status = status;
                usuarioSecretaria.IsAtivo = true;

                await _repository.AtualizarUsuarioSecretaria(usuarioSecretaria);
                await _repository.SaveChangesAsync();

                return Result<UsuarioSecretariaResponse>.Success(MapearResponse(usuarioSecretaria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do usuário da secretaria com ID {UsuarioSecretariaId}", id);
                return Result<UsuarioSecretariaResponse>.Failure("Erro ao alterar status do usuário da secretaria");
            }
        }

        private static UsuarioSecretariaResponse MapearResponse(UsuarioSecretaria usuarioSecretaria)
        {
            return new UsuarioSecretariaResponse
            {
                Id = usuarioSecretaria.Id,
                EmailUsuario = usuarioSecretaria.EmailUsuario,
                EmailConfirmado = usuarioSecretaria.EmailConfirmado,
                RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                Chave = usuarioSecretaria.Chave,
                NomeUsuario = usuarioSecretaria.NomeUsuario,
                IsAtivo = usuarioSecretaria.IsAtivo,
                Status = usuarioSecretaria.Status.ToString(),
                InstituicaoId = usuarioSecretaria.InstituicaoId
            };
        }

        private async Task<Result<bool>> EnviarEmailConfirmacaoConta(string email, string nome, string chave)
        {
            if (_emailService is null)
            {
                return Result<bool>.Success(true);
            }

            var envio = await _emailService.EnviarEmailConfirmacaoConta(email, nome, chave);
            if (envio.IsFailure)
            {
                _logger.LogWarning("Não foi possível enviar e-mail de confirmação para {Email}", email);
                return Result<bool>.Failure("Não foi possível enviar o e-mail de confirmação. Verifique o endereço institucional e tente novamente.");
            }

            return envio;
        }
    }
}
