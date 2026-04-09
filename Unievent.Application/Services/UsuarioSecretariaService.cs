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
        public UsuarioSecretariaService(IUsuarioSecretariaRepository repository)
        {
            _repository = repository;
        }


        async Task<ResultData<UsuarioSecretariaResponse>> IUsuarioSecretariaService.AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update)
        {
            var emailExistente = await _repository.ListarUsuarioSecretariaByEmail(update.EmailUsuario);
            var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
            if (usuarioSecretaria is null)
            {
                return ResultData<UsuarioSecretariaResponse>.Failure("UsuarioSecretaria não encontrado");
            }
            if (!string.IsNullOrWhiteSpace(update.EmailUsuario) && emailExistente == null)
            {

                usuarioSecretaria.EmailUsuario = update.EmailUsuario;
            }
            else if (!string.IsNullOrWhiteSpace(update.EmailUsuario) && emailExistente != null)
            {
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

        async Task<ResultData<UsuarioSecretariaResponse>> IUsuarioSecretariaService.CriarUsuarioSecretaria(UsuarioSecretariaRequest request)
        {
            var role = Enum.TryParse(request.RoleUsuario, out Role result) ? result : Role.Admin;
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var emailExistente = await _repository.ListarUsuarioSecretariaByEmail(request.EmailUsuario);
            if (emailExistente != null)
            {
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

        async Task<Result> IUsuarioSecretariaService.DeletarUsuarioSecretaria(int id)
        {
            var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
            if (usuarioSecretaria is null)
            {
                return Result.Failure("UsuarioSecretaria não encontrado");
            }
            usuarioSecretaria.IsAtivo = false;
            await _repository.SaveChangesAsync();
            return Result.Success("UsuarioSecretaria deletado com sucesso");
        }

        async Task<ResultData<IEnumerable<UsuarioSecretariaResponse>>> IUsuarioSecretariaService.ListarUsuarioSecretaria()
        {
            var usuarioSecretarias = await _repository.ListarUsuarioSecretarias();
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

        async Task<ResultData<UsuarioSecretariaResponse>> IUsuarioSecretariaService.ListarUsuarioSecretariaById(int id)
        {
            var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id);
            if (usuarioSecretaria is null)
            {
                return ResultData<UsuarioSecretariaResponse>.Failure("UsuarioSecretaria não encontrado");
            }

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

        async Task<ResultData<UsuarioSecretariaLoginResponse>> IUsuarioSecretariaService.Login(UsuarioSecretariaLoginRequest request)
        {
            var usuarioSecretaria = _repository.ListarUsuarioSecretariaByEmail(request.Email).Result;
            if (usuarioSecretaria is null)
            {
                return ResultData<UsuarioSecretariaLoginResponse>.Failure("UsuarioSecretaria não encontrado");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuarioSecretaria.Senha))
            {
                return ResultData<UsuarioSecretariaLoginResponse>.Failure("Senha incorreta");
            }
            return await Task.FromResult(ResultData<UsuarioSecretariaLoginResponse>.Success(new UsuarioSecretariaLoginResponse
            {
                Token = "sjikjd"

            }));
        }
    }
}
