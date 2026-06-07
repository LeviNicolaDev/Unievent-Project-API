using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _service;

        public EmailController(IEmailService emailService)
        {
            _service = emailService;
        }

        [HttpPost("enviar-email-confirmacao-conta")]
        public async Task<IActionResult> EnviarEmailConfirmacaoContaAsync([FromBody] EmailConfirmacaoRequest request)
        {
            var response = await _service.EnviarEmailConfirmacaoConta(request.Email, request.Nome, request.Chave);
            if (response.IsFailure)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Value);
        }
    }
}
