using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Enuns;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController(IEventoService _service) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> CriarEvento([FromForm] EventoRequest request)
        {
            var response = await _service.CriarEvento(request);
            if (response.IsFailure)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> AtualizarEvento(int id, [FromForm] EventoUpdate update)
        {
            var response = await _service.AtualizarEvento(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpGet]

        public async Task<IActionResult> ListarEventos()
        {
            var response = await _service.ListarEventos();
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarEventoById(int id)
        {
            var response = await _service.ListarEventoById(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> DeletarEvento(int id)
        {

            var response = await _service.DeletarEvento(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategorias()
        {
            var result = Enum.GetNames(typeof(Categoria));
            return Ok(result);
        }

        [HttpGet("eventos")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<ActionResult<IList<EventoResponse>>> GetEventosByCategoria([FromQuery] Categoria? categoria)
        {
            if (categoria is null)
                return BadRequest("Categoria é obrigatória");

            var result = await _service.ListarEventosByCategoria(categoria.Value);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }
    }
}