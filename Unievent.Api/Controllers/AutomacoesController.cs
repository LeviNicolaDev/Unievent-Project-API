using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/admin/automacoes")]
[Authorize(Roles = "Admin,Secretaria")]
public class AutomacoesController(IAutomacaoEventosService automacaoEventosService) : ControllerBase
{
    [HttpPost("processar-eventos")]
    public async Task<IActionResult> ProcessarEventos(CancellationToken cancellationToken)
    {
        var result = await automacaoEventosService.ProcessarAsync(cancellationToken);
        return result.IsFailure ? BadRequest(result.Errors) : Ok(result.Value);
    }
}
