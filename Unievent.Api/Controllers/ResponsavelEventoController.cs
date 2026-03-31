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
        public async Task<IActionResult> CriarResponsavelEvento([FromBody] ResponsavelEventoRequest request)
        {
            var response = await _service.CriarResponsavelEvento(request);
            return Ok(response);
        }

        [HttpPut("AtualizarResponsavelEvento/{id}")]
        public async Task<IActionResult> AtualizarResponsavelEvento(int id, [FromBody] ResponsavelEventoUpdate update)
        {
            var response = await _service.AtualizarResponsavelEvento(id, update);
            return Ok(response);
        }

        [HttpGet("ListarResponsaveisEvento")]
        public async Task<IActionResult> ListarResponsaveisEvento()
        {
            var response = await _service.ListarResponsaveisEvento();
            return Ok(response);
        }

        [HttpGet("ListarResponsavelEventoById/{id}")]
        public async Task<IActionResult> ListarResponsavelEventoById(int id)
        {
            var response = await _service.ListarResponsavelEventoById(id);
            return Ok(response);
        }

        [HttpDelete("DeletarResponsavelEvento/{id}")]
        public async Task<IActionResult> DeletarResponsavelEvento(int id)
        {
            var response = await _service.DeletarResponsavelEvento(id);
            return Ok(response);
        }
    }
}
