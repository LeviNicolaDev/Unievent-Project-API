using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Api.Security;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioSecretariaController(IUsuarioSecretariaService _service, AppDbContext db) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CriarUsuarioSecretaria([FromBody] UsuarioSecretariaRequest request)
        {
            if (request.RoleUsuario != Role.Secretaria)
                return BadRequest("UsuarioSecretaria deve ser cadastrado com perfil Secretaria");

            if (!User.IsGlobalAdmin()) return Forbid();

            if (User.IsGlobalAdmin())
            {
                if (!request.InstituicaoId.HasValue)
                    return BadRequest("A instituição é obrigatória para criar usuário Secretaria");
            }
            request.Status = StatusUsuarioSecretaria.Ativo;

            var response = await _service.CriarUsuarioSecretaria(request);
            if (response.IsFailure)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpPost("solicitar-cadastro")]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarCadastroSecretaria([FromBody] UsuarioSecretariaRequest request)
        {
            request.RoleUsuario = Role.Secretaria;
            request.Status = StatusUsuarioSecretaria.Pendente;
            request.Chave = string.IsNullOrWhiteSpace(request.Chave) ? Guid.NewGuid().ToString("N") : request.Chave;

            if (!request.InstituicaoId.HasValue)
                return BadRequest("Selecione uma instituição para solicitar o cadastro de Secretaria");

            var response = await _service.CriarUsuarioSecretaria(request);
            if (response.IsFailure)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response.Value);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> AtualizarUsuarioSecretaria(int id, [FromBody] UsuarioSecretariaUpdate update)
        {
            if (!await PodeAcessarUsuarioSecretaria(id)) return Forbid();
            if (update.InstituicaoId.HasValue && !User.CanAccessInstituicao(update.InstituicaoId)) return Forbid();
            if (!User.IsGlobalAdmin()) update.InstituicaoId = User.GetInstituicaoId();

            var response = await _service.AtualizarUsuarioSecretaria(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);

        }

        [HttpPatch("{id}/aprovar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AprovarUsuarioSecretaria(int id)
        {
            if (!User.IsGlobalAdmin()) return Forbid();

            var response = await _service.AlterarStatusUsuarioSecretaria(id, StatusUsuarioSecretaria.Ativo);
            if (response.IsFailure) return NotFound(response.Errors);

            return Ok(response.Value);
        }

        [HttpPatch("{id}/recusar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RecusarUsuarioSecretaria(int id)
        {
            if (!User.IsGlobalAdmin()) return Forbid();

            var response = await _service.AlterarStatusUsuarioSecretaria(id, StatusUsuarioSecretaria.Recusado);
            if (response.IsFailure) return NotFound(response.Errors);

            return Ok(response.Value);
        }

        [HttpPatch("{id}/bloquear")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BloquearUsuarioSecretaria(int id)
        {
            if (!User.IsGlobalAdmin()) return Forbid();

            var response = await _service.AlterarStatusUsuarioSecretaria(id, StatusUsuarioSecretaria.Bloqueado);
            if (response.IsFailure) return NotFound(response.Errors);

            return Ok(response.Value);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarUsuarioSecretaria()
        {
            var response = await _service.ListarUsuarioSecretaria();
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            var usuarios = response.Value ?? [];
            if (!User.IsGlobalAdmin())
                usuarios = usuarios.Where(u => User.CanAccessInstituicao(u.InstituicaoId));

            return Ok(usuarios);

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarUsuarioSecretariaById(int id)
        {
            if (!await PodeAcessarUsuarioSecretaria(id)) return Forbid();
            var response = await _service.ListarUsuarioSecretariaById(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> DeletarUsuarioSecretaria(int id)
        {
            if (!await PodeAcessarUsuarioSecretaria(id)) return Forbid();
            var response = await _service.DeletarUsuarioSecretaria(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        private async Task<bool> PodeAcessarUsuarioSecretaria(int id)
        {
            var usuario = await db.UsuarioSecretaria.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            return usuario is not null && User.CanAccessInstituicao(usuario.InstituicaoId);
        }
    }
}
