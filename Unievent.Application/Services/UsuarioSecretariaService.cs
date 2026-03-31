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


        async Task<UsuarioSecretariaResponse> IUsuarioSecretariaService.AtualizarUsuarioSecretaria(int id, UsuarioSecretariaUpdate update)
        {

            var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id) ?? throw new Exception(" UsuarioSecretaria não encontrado");
            if (!string.IsNullOrWhiteSpace(update.EmailUsuario))
            {

                usuarioSecretaria.EmailUsuario = update.EmailUsuario;
            }

            if (!string.IsNullOrWhiteSpace(update.NomeUsuario))
            {

                usuarioSecretaria.NomeUsuario = update.NomeUsuario;
            }

            /* if (!string.IsNullOrWhiteSpace(update.IsAtivo) && update.IsAtivo.Equals("ativo", StringComparison.CurrentCultureIgnoreCase))
             {
                 usuarioSecretaria.IsAtivo = true;
                 // var status = Enum.TryParse(update.Status, out Situacao situacao) ? situacao : Situacao.inativo;

             }*/

            await _repository.AtualizarUsuarioSecretaria(usuarioSecretaria);
            await _repository.SaveChangesAsync();
            return new UsuarioSecretariaResponse
            {
                Id = usuarioSecretaria.Id,
                EmailUsuario = usuarioSecretaria.EmailUsuario,
                RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                Chave = usuarioSecretaria.Chave,
                NomeUsuario = usuarioSecretaria.NomeUsuario,
                IsAtivo = usuarioSecretaria.IsAtivo
            };
        }

        async Task<UsuarioSecretariaResponse> IUsuarioSecretariaService.CriarUsuarioSecretaria(UsuarioSecretariaRequest request)
        {
            var role = Enum.TryParse(request.RoleUsuario, out Role result) ? result : Role.Admin;
            var status = Enum.TryParse(request.RoleUsuario, out Situacao situacao) ? situacao : Situacao.Inativo;
            var usuarioExiste = await _repository.ListarUsuarioSecretariaByEmail(request.EmailUsuario);

            var usuarioSecretaria = new UsuarioSecretaria
            {
                NomeUsuario = request.NomeUsuario,
                Chave = request.Chave,
                EmailUsuario = request.EmailUsuario,
                RoleUsuario = role,
                IsAtivo = true
            };
            await _repository.CriarUsuarioSecretaria(usuarioSecretaria);
            await _repository.SaveChangesAsync();
            return new UsuarioSecretariaResponse
            {
                Id = usuarioSecretaria.Id,
                EmailUsuario = usuarioSecretaria.EmailUsuario,
                RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                Chave = usuarioSecretaria.Chave,
                NomeUsuario = usuarioSecretaria.NomeUsuario,
                IsAtivo = usuarioSecretaria.IsAtivo
            };
        }

        async Task<bool> IUsuarioSecretariaService.DeletarUsuarioSecretaria(int id)
        {
            var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id) ?? throw new Exception(" UsuarioSecretaria não encontrado");
            usuarioSecretaria.IsAtivo = false;
            await _repository.SaveChangesAsync();
            return true;
        }

        async Task<IEnumerable<UsuarioSecretariaResponse>> IUsuarioSecretariaService.ListarUsuarioSecretaria()
        {
            var usuarioSecretarias = await _repository.ListarUsuarioSecretarias();
            return usuarioSecretarias.Select(usuarioSecretaria => new UsuarioSecretariaResponse
            {
                Id = usuarioSecretaria.Id,
                EmailUsuario = usuarioSecretaria.EmailUsuario,
                RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                Chave = usuarioSecretaria.Chave,
                NomeUsuario = usuarioSecretaria.NomeUsuario,
                IsAtivo = usuarioSecretaria.IsAtivo
            });
        }

        async Task<UsuarioSecretariaResponse> IUsuarioSecretariaService.ListarUsuarioSecretariaById(int id)
        {
            var usuarioSecretaria = await _repository.ListarUsuarioSecretariaById(id) ?? throw new Exception(" UsuarioSecretaria não encontrado");

            return new UsuarioSecretariaResponse
            {
                Id = usuarioSecretaria.Id,
                EmailUsuario = usuarioSecretaria.EmailUsuario,
                RoleUsuario = usuarioSecretaria.RoleUsuario.ToString(),
                Chave = usuarioSecretaria.Chave,
                NomeUsuario = usuarioSecretaria.NomeUsuario,
                IsAtivo = usuarioSecretaria.IsAtivo
            };
        }
    }
}
