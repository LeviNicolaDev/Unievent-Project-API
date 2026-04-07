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
            return Ok(response);
        }

        [HttpPut("AtualizarEvento/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AtualizarEvento(int id, [FromForm] EventoUpdate update)
        {
            var response = await _service.AtualizarEvento(id, update);
            return Ok(response);
        }

        [HttpGet("ListarEventos")]
        public async Task<IActionResult> ListarEventos()
        {
            var response = await _service.ListarEventos();
            return Ok(response);
        }

        [HttpGet("ListarEventoById/{id}")]
        public async Task<IActionResult> ListarEventoById(int id)
        {
            var response = await _service.ListarEventoById(id);
            return Ok(response);
        }

        [HttpDelete("DeletarEvento/{id}")]
        public async Task<IActionResult> DeletarEvento(int id)
        {
            var response = await _service.DeletarEvento(id);
            return Ok(response);
        }
    }
}