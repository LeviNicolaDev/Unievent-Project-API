using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Services;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _service;

        public EmailController(EmailService emailService)
        {
            _service = emailService;
        }

        [HttpPost("enviar-email-confirmacao-conta")]
        public async Task<IActionResult> EnviarEmailConfirmacaoContaAsync([FromBody] string email, string nome, string chave)
        {
            var response = await _service.EnviarEmailConfirmacaoConta(email, nome, chave);
            if (response.IsFailure)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Value);
        }
    }
}
