using Microsoft.AspNetCore.Authorization;
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
            if (response.IsFailure)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpPut("AtualizarUsuarioSecretaria/{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> AtualizarUsuarioSecretaria(int id, [FromBody] UsuarioSecretariaUpdate update)
        {
            var response = await _service.AtualizarUsuarioSecretaria(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);

        }

        [HttpGet("ListarUsuarioSecretaria")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarUsuarioSecretaria()
        {
            var response = await _service.ListarUsuarioSecretaria();
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);

        }

        [HttpGet("ListarUsuarioSecretariaById/{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarUsuarioSecretariaById(int id)
        {
            var response = await _service.ListarUsuarioSecretariaById(id);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpDelete("DeletarUsuarioSecretaria/{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> DeletarUsuarioSecretaria(int id)
        {
            var response = await _service.DeletarUsuarioSecretaria(id);
            if (response.IsFailure)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Message);
        }
    }
}
