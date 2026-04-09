using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController(IEventoService _service) : ControllerBase
    {
        [HttpPost("CriarEvento")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CriarEvento([FromForm] EventoRequest request)
        {
            var response = await _service.CriarEvento(request);
            if (response.IsFailure)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpPut("AtualizarEvento/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AtualizarEvento(int id, [FromForm] EventoUpdate update)
        {
            var response = await _service.AtualizarEvento(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpGet("ListarEventos")]
        public async Task<IActionResult> ListarEventos()
        {
            var response = await _service.ListarEventos();
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpGet("ListarEventoById/{id}")]
        public async Task<IActionResult> ListarEventoById(int id)
        {
            var response = await _service.ListarEventoById(id);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpDelete("DeletarEvento/{id}")]
        public async Task<IActionResult> DeletarEvento(int id)
        {
            var response = await _service.DeletarEvento(id);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Message);
        }

        [HttpGet("ListarEventosByCategoria/{categoria}")]
        public async Task<IActionResult> ListarEventosByCategoria([FromRoute] string categoria)
        {
            var response = await _service.ListarEventosByCategoria(categoria);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }
    }
}