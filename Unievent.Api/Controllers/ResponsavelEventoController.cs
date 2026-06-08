using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.ResponsavelEvento;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsavelEventoController(IResponsavelEventoService _service) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> CriarResponsavelEvento([FromForm] ResponsavelEventoRequest request)
        {
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
            return Ok(response.Value);

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarResponsavelEventoById(int id)
        {
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
            var response = await _service.DeletarResponsavelEvento(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Errors);
        }
    }
}
