using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/admin/automacoes")]
[Authorize(Roles = "Admin")]
public class AutomacoesController(IAutomacaoEventosService automacaoEventosService) : ControllerBase
{
    [HttpPost("processar-eventos")]
    public async Task<IActionResult> ProcessarEventos(CancellationToken cancellationToken)
    {
        if (!Unievent.Api.Security.InstitutionalAccess.IsGlobalAdmin(User)) return Forbid();
        var result = await automacaoEventosService.ProcessarAsync(cancellationToken);
        return result.IsFailure ? BadRequest(result.Errors) : Ok(result.Value);
    }
}
