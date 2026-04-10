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
        public UsuarioSecretariaService(IUsuarioSecretariaRepository repository, ILogger<UsuarioSecretariaService> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        async Task<ResultData<UsuarioSecretariaResponse>> IUsuarioSecretariaService.AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update)
        {
            try
            {
                _logger.LogInformation("Iniciando atualização do usuário da secretaria com ID {UsuarioSecretariaId}", id);
                var emailExistente = await _repository.ListarUsuarioSecretariaByEmail(update.EmailUsuario);
                var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com ID {UsuarioSecretariaId} não encontrado para atualização", id);
                    return ResultData<UsuarioSecretariaResponse>.Failure("UsuarioSecretaria não encontrado");
                }
                if (!string.IsNullOrWhiteSpace(update.EmailUsuario) && emailExistente == null)
                {
                    usuarioSecretaria.EmailUsuario = update.EmailUsuario;
                }
                else if (!string.IsNullOrWhiteSpace(update.EmailUsuario) && emailExistente != null)
                {
                    _logger.LogWarning("Email {Email} já cadastrado para outro usuário da secretaria", update.EmailUsuario);
                    return ResultData<UsuarioSecretariaResponse>.Failure("Email já cadastrado para outro usuário da secretaria");
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

                /* if (!string.IsNullOrWhiteSpace(update.IsAtivo) && update.IsAtivo.Equals("ativo", StringComparison.CurrentCultureIgnoreCase))
                 {
                     usuarioSecretaria.IsAtivo = true;
                     // var status = Enum.TryParse(update.Status, out Situacao situacao) ? situacao : Situacao.inativo;

                 }*/

                await _repository.AtualizarUsuarioSecretaria(usuarioSecretaria);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Usuário da secretaria com ID {UsuarioSecretariaId} atualizado com sucesso", id);
                return ResultData<UsuarioSecretariaResponse>.Success(new UsuarioSecretariaResponse
                {
                    Id = usuarioSecretaria.Id,
                    EmailUsuario = usuarioSecretaria.EmailUsuario,
                    RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),

                    Chave = usuarioSecretaria.Chave,
                    NomeUsuario = usuarioSecretaria.NomeUsuario,
                    IsAtivo = usuarioSecretaria.IsAtivo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário da secretaria com ID {UsuarioSecretariaId}", id);
                return ResultData<UsuarioSecretariaResponse>.Failure($"Erro ao atualizar usuário da secretaria");
            }
        }

        async Task<ResultData<UsuarioSecretariaResponse>> IUsuarioSecretariaService.CriarUsuarioSecretaria(UsuarioSecretariaRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando criação do usuário da secretaria com email {EmailUsuario}", request.EmailUsuario);
                var role = Enum.TryParse(request.RoleUsuario, out Role result) ? result : Role.Secretaria;
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
                var emailExistente = await _repository.ListarUsuarioSecretariaByEmail(request.EmailUsuario);
                if (emailExistente != null)
                {
                    _logger.LogWarning("Email {EmailUsuario} já cadastrado para outro usuário da secretaria", request.EmailUsuario);
                    return ResultData<UsuarioSecretariaResponse>.Failure("Email já cadastrado para outro usuário da secretaria");
                }
                var usuarioSecretaria = new UsuarioSecretaria
                {
                    NomeUsuario = request.NomeUsuario,
                    Chave = request.Chave,
                    Senha = senhaHash,
                    EmailUsuario = request.EmailUsuario,
                    RoleUsuario = role,
                    IsAtivo = true
                };
                await _repository.CriarUsuarioSecretaria(usuarioSecretaria);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Usuário da secretaria com email {EmailUsuario} criado com sucesso", request.EmailUsuario);
                return ResultData<UsuarioSecretariaResponse>.Success(new UsuarioSecretariaResponse
                {
                    Id = usuarioSecretaria.Id,
                    EmailUsuario = usuarioSecretaria.EmailUsuario,
                    RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                    Chave = usuarioSecretaria.Chave,
                    NomeUsuario = usuarioSecretaria.NomeUsuario,
                    IsAtivo = usuarioSecretaria.IsAtivo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário da secretaria com email {EmailUsuario}", request.EmailUsuario);
                return ResultData<UsuarioSecretariaResponse>.Failure("Erro ao criar usuário da secretaria");
            }
        }

        async Task<Result> IUsuarioSecretariaService.DeletarUsuarioSecretaria(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando deleção do usuário da secretaria com ID {UsuarioSecretariaId}", id);
                var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com ID {UsuarioSecretariaId} não encontrado para deleção", id);
                    return Result.Failure("UsuarioSecretaria não encontrado");
                }
                usuarioSecretaria.IsAtivo = false;
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Usuário da secretaria com ID {UsuarioSecretariaId} deletado com sucesso", id);
                return Result.Success("UsuarioSecretaria deletado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário da secretaria com ID {UsuarioSecretariaId}", id);
                return Result.Failure("Erro ao deletar usuário da secretaria");
            }
        }

        async Task<ResultData<IEnumerable<UsuarioSecretariaResponse>>> IUsuarioSecretariaService.ListarUsuarioSecretaria()
        {
            try
            {
                _logger.LogInformation("Iniciando listagem de usuários da secretaria");
                var usuarioSecretarias = await _repository.ListarUsuarioSecretarias();
                _logger.LogInformation("Usuários da secretaria listados com sucesso {UsuarioSecretariasCount}", usuarioSecretarias.Count());
                return ResultData<IEnumerable<UsuarioSecretariaResponse>>.Success(usuarioSecretarias.Select(usuarioSecretaria => new UsuarioSecretariaResponse
                {
                    Id = usuarioSecretaria.Id,
                    EmailUsuario = usuarioSecretaria.EmailUsuario,
                    RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                    Chave = usuarioSecretaria.Chave,
                    NomeUsuario = usuarioSecretaria.NomeUsuario,
                    IsAtivo = usuarioSecretaria.IsAtivo
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários da secretaria");
                return ResultData<IEnumerable<UsuarioSecretariaResponse>>.Failure("Erro ao listar usuários da secretaria");
            }
        }

        async Task<ResultData<UsuarioSecretariaResponse>> IUsuarioSecretariaService.ListarUsuarioSecretariaById(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando busca do usuário da secretaria com ID {UsuarioSecretariaId}", id);
                var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com ID {UsuarioSecretariaId} não encontrado", id);
                    return ResultData<UsuarioSecretariaResponse>.Failure("UsuarioSecretaria não encontrado");
                }
                _logger.LogInformation("Usuário da secretaria com ID {UsuarioSecretariaId} encontrado com sucesso", id);
                return ResultData<UsuarioSecretariaResponse>.Success(new UsuarioSecretariaResponse
                {
                    Id = usuarioSecretaria.Id,
                    EmailUsuario = usuarioSecretaria.EmailUsuario,
                    RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                    Chave = usuarioSecretaria.Chave,
                    NomeUsuario = usuarioSecretaria.NomeUsuario,
                    IsAtivo = usuarioSecretaria.IsAtivo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário da secretaria com ID {UsuarioSecretariaId}", id);
                return ResultData<UsuarioSecretariaResponse>.Failure("Erro ao buscar usuário da secretaria");
            }
        }

        async Task<ResultData<UsuarioSecretariaLoginResponse>> IUsuarioSecretariaService.Login(UsuarioSecretariaLoginRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando login do usuário da secretaria com email {EmailUsuario}", request.Email);
                var usuarioSecretaria = _repository.ListarUsuarioSecretariaByEmail(request.Email).Result;
                if (usuarioSecretaria is null)
                {
                    _logger.LogWarning("Usuário da secretaria com email {EmailUsuario} não encontrado para login", request.Email);
                    return ResultData<UsuarioSecretariaLoginResponse>.Failure("UsuarioSecretaria não encontrado");
                }
                if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuarioSecretaria.Senha))
                {
                    _logger.LogWarning("Tentativa de login com email {EmailUsuario} falhou: senha incorreta", request.Email);
                    return ResultData<UsuarioSecretariaLoginResponse>.Failure("Senha incorreta");
                }
                _logger.LogInformation("Usuário da secretaria com email {EmailUsuario} logado com sucesso", request.Email);
                return await Task.FromResult(ResultData<UsuarioSecretariaLoginResponse>.Success(new UsuarioSecretariaLoginResponse
                {

                    Token = "sjikjd"

                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login do usuário da secretaria com email {EmailUsuario}", request.Email);
                return ResultData<UsuarioSecretariaLoginResponse>.Failure("Erro ao realizar login do usuário da secretaria");
            }
        }
    }
}
