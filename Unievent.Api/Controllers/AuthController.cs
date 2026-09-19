using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Auth;
using Unievent.Application.Dtos.UsuarioSecretaria;

using Unievent.Application.Services;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioSecretariaLoginRequest request)
    {
        var result = await _authService.Login(request);
        if (!result.IsSuccess)
            return Unauthorized(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("login-aluno")]
    public async Task<IActionResult> LoginAluno([FromBody] Application.Dtos.Aluno.LoginRequest request)
    {
        var result = await _authService.LoginAluno(request);
        if (!result.IsSuccess)
            return Unauthorized(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("cadastro-publico")]
    public async Task<IActionResult> CadastrarPublicoGeral([FromBody] PublicoGeralCadastroRequest request)
    {
        var result = await _authService.CadastrarPublicoGeral(request);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    [HttpPost("login-publico")]
    public async Task<IActionResult> LoginPublicoGeral([FromBody] Application.Dtos.Aluno.LoginRequest request)
    {
        var result = await _authService.LoginPublicoGeral(request);
        if (!result.IsSuccess)
            return Unauthorized(result.Errors);

        return Ok(result.Value);
    }
}
