using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioSecretariaController(IUsuarioSecretariaService _service) : ControllerBase
    {
        [HttpPost("CriarUsuarioSecretaria")]
        public async Task<IActionResult> CriarUsuarioSecretaria([FromBody] UsuarioSecretariaRequest request)
        {
            var response = await _service.CriarUsuarioSecretaria(request);
            return Ok(response);
        }

        [HttpPut("AtualizarUsuarioSecretaria/{id}")]
        public async Task<IActionResult> AtualizarUsuarioSecretaria(int id, [FromBody] UsuarioSecretariaUpdate update)
        {
            var response = await _service.AtualizarUsuarioSecretaria(id, update);
            return Ok(response);
        }

        [HttpGet("ListarUsuarioSecretaria")]
        public async Task<IActionResult> ListarUsuarioSecretaria()
        {
            var response = await _service.ListarUsuarioSecretaria();
            return Ok(response);
        }

        [HttpGet("ListarUsuarioSecretariaById/{id}")]
        public async Task<IActionResult> ListarUsuarioSecretariaById(int id)
        {
            var response = await _service.ListarUsuarioSecretariaById(id);
            return Ok(response);
        }

        [HttpDelete("DeletarUsuarioSecretaria/{id}")]
        public async Task<IActionResult> DeletarUsuarioSecretaria(int id)
        {
            var response = await _service.DeletarUsuarioSecretaria(id);
            return Ok(response);
        }
    }
}
