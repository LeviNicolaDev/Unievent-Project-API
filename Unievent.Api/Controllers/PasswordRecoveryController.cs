using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Unievent.Application.Configurations.Email;
using Unievent.Infra.Services;

namespace Unievent.Api.Controllers;

[ApiController, AllowAnonymous, Route("api/Auth"), EnableRateLimiting("password-recovery")]
public class PasswordRecoveryController(PasswordRecoveryService service, IOptions<EmailSettings> settings) : ControllerBase
{
    public record SolicitarRequest([Required, EmailAddress, StringLength(254)] string Email,
        [Required, RegularExpression("^(administrativo|publico|mobile)$")] string Perfil = "publico");
    public record RedefinirRequest([Required, StringLength(64, MinimumLength = 64)] string Token,
        [Required, StringLength(72, MinimumLength = 8)] string Senha, [Required] string ConfirmacaoSenha);

    [HttpPost("solicitar-recuperacao-senha")]
    public async Task<IActionResult> Solicitar(SolicitarRequest request)
    {
        await service.SolicitarAsync(request.Email, request.Perfil);
        return Ok(new { mensagem = PasswordRecoveryService.MensagemSolicitacao });
    }

    [HttpPost("redefinir-senha")]
    public async Task<IActionResult> Redefinir(RedefinirRequest request)
    {
        var result = await service.RedefinirAsync(request.Token, request.Senha, request.ConfirmacaoSenha);
        if (result.IsFailure) return BadRequest(result.Errors);
        var tipo = result.Value;
        var destino = tipo == "Aluno" ? settings.Value.MobileLoginDeepLink : tipo == "Publico" ? "/entrar" : "/login";
        return Ok(new { mensagem = "Senha redefinida com sucesso. Faça login com sua nova senha.", tipoUsuario = tipo, destino });
    }
}
