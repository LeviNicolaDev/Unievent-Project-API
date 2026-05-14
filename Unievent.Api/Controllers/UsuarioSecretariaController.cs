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
        [HttpPost]

        public async Task<IActionResult> CriarUsuarioSecretaria([FromBody] UsuarioSecretariaRequest request)
        {
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
            var response = await _service.AtualizarUsuarioSecretaria(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
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
            return Ok(response.Value);

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> ListarUsuarioSecretariaById(int id)
        {
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
            var response = await _service.DeletarUsuarioSecretaria(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }
    }
}
