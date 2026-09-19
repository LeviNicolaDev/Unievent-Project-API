using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unievent.Api.Security;
using Unievent.Application.Dtos.UsuarioUnievent;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioUnieventController : ControllerBase
{
    private readonly IUsuarioUnieventRepository _usuarioUnieventRepository;
    private readonly IUsuarioSecretariaRepository _usuarioSecretariaRepository;

    public UsuarioUnieventController(
        IUsuarioUnieventRepository usuarioUnieventRepository,
        IUsuarioSecretariaRepository usuarioSecretariaRepository)
    {
        _usuarioUnieventRepository = usuarioUnieventRepository;
        _usuarioSecretariaRepository = usuarioSecretariaRepository;
    }

    [HttpPost("admin-inicial")]
    [AllowAnonymous]
    public async Task<IActionResult> CriarAdminInicial([FromBody] UsuarioUnieventRequest request)
    {
        if (await _usuarioUnieventRepository.ExisteUsuarioUnievent())
            return Conflict("O Admin UniEvent inicial já foi cadastrado");

        var erro = ValidarRequest(request);
        if (erro is not null) return BadRequest(erro);

        if (await EmailJaExiste(request.EmailUsuario))
            return BadRequest("Email já cadastrado");

        var usuarioUnievent = CriarEntidade(request);
        await _usuarioUnieventRepository.CriarUsuarioUnievent(usuarioUnievent);
        await _usuarioUnieventRepository.SaveChangesAsync();

        return Ok(MapearResponse(usuarioUnievent));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarAdminUnievent([FromBody] UsuarioUnieventRequest request)
    {
        if (!User.IsGlobalAdmin()) return Forbid();

        var erro = ValidarRequest(request);
        if (erro is not null) return BadRequest(erro);

        if (await EmailJaExiste(request.EmailUsuario))
            return BadRequest("Email já cadastrado");

        var usuarioUnievent = CriarEntidade(request);
        await _usuarioUnieventRepository.CriarUsuarioUnievent(usuarioUnievent);
        await _usuarioUnieventRepository.SaveChangesAsync();

        return Ok(MapearResponse(usuarioUnievent));
    }

    private async Task<bool> EmailJaExiste(string email)
    {
        return await _usuarioUnieventRepository.ListarUsuarioUnieventByEmail(email) is not null ||
               await _usuarioSecretariaRepository.ListarUsuarioSecretariaByEmail(email) is not null;
    }

    private static string? ValidarRequest(UsuarioUnieventRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NomeUsuario)) return "O nome deve ser preenchido";
        if (string.IsNullOrWhiteSpace(request.EmailUsuario)) return "O email deve ser preenchido";
        if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 6) return "A senha deve conter no mínimo 6 caracteres";

        return null;
    }

    private static UsuarioUnievent CriarEntidade(UsuarioUnieventRequest request)
    {
        return new UsuarioUnievent
        {
            NomeUsuario = request.NomeUsuario,
            EmailUsuario = request.EmailUsuario,
            Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Chave = request.Chave ?? string.Empty,
            RoleUsuario = Role.Admin,
            IsAtivo = true
        };
    }

    private static UsuarioUnieventResponse MapearResponse(UsuarioUnievent usuarioUnievent)
    {
        return new UsuarioUnieventResponse
        {
            Id = usuarioUnievent.Id,
            NomeUsuario = usuarioUnievent.NomeUsuario,
            EmailUsuario = usuarioUnievent.EmailUsuario,
            RoleUsuario = usuarioUnievent.RoleUsuario.ToString(),
            IsAtivo = usuarioUnievent.IsAtivo
        };
    }
}
