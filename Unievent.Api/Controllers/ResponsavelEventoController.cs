using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Api.Security;
using Unievent.Application.Dtos.ResponsavelEvento;
using Unievent.Application.Interfaces.Services;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsavelEventoController(IResponsavelEventoService _service, AppDbContext db) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> CriarResponsavelEvento([FromForm] ResponsavelEventoRequest request)
        {
            if (User.IsGlobalAdmin())
            {
                if (!request.InstituicaoId.HasValue)
                    return BadRequest("A instituição é obrigatória para criar responsável por evento");
            }
            else
            {
                var instituicaoId = User.GetInstituicaoId();
                if (!instituicaoId.HasValue) return Forbid();
                if (request.InstituicaoId.HasValue && request.InstituicaoId != instituicaoId) return Forbid();
                request.InstituicaoId = instituicaoId;
            }

            var response = await _service.CriarResponsavelEvento(request);
            if (response.IsFailure)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Value);

        }

        [HttpPatch("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> AtualizarResponsavelEvento(int id, [FromForm] ResponsavelEventoUpdate update)
        {
            if (!await PodeAcessarResponsavel(id)) return Forbid();
            if (update.InstituicaoId.HasValue && !User.CanAccessInstituicao(update.InstituicaoId)) return Forbid();
            if (!User.IsGlobalAdmin()) update.InstituicaoId = User.GetInstituicaoId();

            var response = await _service.AtualizarResponsavelEvento(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);

        }

        [HttpGet]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarResponsaveisEvento()
        {
            var response = await _service.ListarResponsaveisEvento();
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            var responsaveis = response.Value ?? [];
            if (!User.IsGlobalAdmin())
                responsaveis = responsaveis.Where(r => User.CanAccessInstituicao(r.InstituicaoId));

            return Ok(responsaveis);

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarResponsavelEventoById(int id)
        {
            if (!await PodeAcessarResponsavel(id)) return Forbid();
            var response = await _service.ListarResponsavelEventoById(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> DeletarResponsavelEvento(int id)
        {
            if (!await PodeAcessarResponsavel(id)) return Forbid();
            var response = await _service.DeletarResponsavelEvento(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        private async Task<bool> PodeAcessarResponsavel(int id)
        {
            var responsavel = await db.ResponsavelEvento.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            return responsavel is not null && User.CanAccessInstituicao(responsavel.InstituicaoId);
        }
    }
}
