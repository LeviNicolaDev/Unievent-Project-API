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
        [HttpPost("CriarResponsavelEvento")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> CriarResponsavelEvento([FromForm] ResponsavelEventoRequest request)
        {
            var response = await _service.CriarResponsavelEvento(request);
            if (response.IsFailure)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);

        }

        [HttpPut("AtualizarResponsavelEvento/{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> AtualizarResponsavelEvento(int id, [FromBody] ResponsavelEventoUpdate update)
        {
            var response = await _service.AtualizarResponsavelEvento(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);

        }

        [HttpGet("ListarResponsaveisEvento")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarResponsaveisEvento()
        {
            var response = await _service.ListarResponsaveisEvento();
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);

        }

        [HttpGet("ListarResponsavelEventoById/{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarResponsavelEventoById(int id)
        {
            var response = await _service.ListarResponsavelEventoById(id);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpDelete("DeletarResponsavelEvento/{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> DeletarResponsavelEvento(int id)
        {
            var response = await _service.DeletarResponsavelEvento(id);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Message);
        }
    }
}
