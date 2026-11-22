using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using Unievent.Application.Configurations.Email;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Infra.Data;
using Unievent.Domain.Enuns;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _service;
        private readonly AppDbContext _db;
        private readonly EmailSettings _emailSettings;

        public EmailController(IEmailService emailService, AppDbContext db, IOptions<EmailSettings> emailSettings)
        {
            _service = emailService;
            _db = db;
            _emailSettings = emailSettings.Value;
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

        [HttpPost("reenviar-confirmacao")]
        public async Task<IActionResult> ReenviarConfirmacao([FromBody] ReenvioConfirmacaoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email)) return BadRequest("Informe o e-mail.");
            var email = request.Email.Trim();
            var admin = await _db.UsuarioUnievent.FirstOrDefaultAsync(u => u.EmailUsuario == email);
            var publico = await _db.Aluno.FirstOrDefaultAsync(a => a.Email == email && a.TipoParticipante == TipoParticipante.Externo);
            var chave = Guid.NewGuid().ToString("N");
            if (admin is { EmailConfirmado: false })
            {
                var envio = await _service.EnviarEmailConfirmacaoConta(admin.EmailUsuario, admin.NomeUsuario, chave);
                if (envio.IsFailure) return BadRequest(envio.Errors);
                admin.Chave = chave;
                await _db.SaveChangesAsync();
            }
            else if (publico is { EmailConfirmado: false })
            {
                var envio = await _service.EnviarEmailConfirmacaoConta(publico.Email, publico.Nome, chave);
                if (envio.IsFailure) return BadRequest(envio.Errors);
                publico.ChaveConfirmacaoEmail = chave;
                await _db.SaveChangesAsync();
            }
            return Ok(new { mensagem = "Se houver uma conta pendente, enviaremos a confirmação por e-mail." });
        }

        public record ReenvioConfirmacaoRequest(string Email);

        [HttpPost("confirmar-conta")]
        public async Task<IActionResult> ConfirmarContaAsync([FromBody] ConfirmarContaRequest request)
        {
            var resultado = await ConfirmarContaPorChave(request.Chave);
            return resultado is null
                ? NotFound("Chave de confirmação inválida ou expirada.")
                : Ok(resultado);
        }

        [HttpGet("confirmar-conta")]
        public async Task<IActionResult> ConfirmarContaPorLinkAsync([FromQuery] string chave)
        {
            var resultado = await ConfirmarContaPorChave(chave);
            if (resultado is null)
            {
                return NotFound("Chave de confirmação inválida ou expirada.");
            }

            if (resultado.TipoUsuario == "Aluno")
            {
                return Content(CriarPaginaRedirecionamentoMobile(resultado.Mensagem), "text/html");
            }

            return Redirect(resultado.Destino);
        }

        private async Task<ConfirmacaoEmailResultado?> ConfirmarContaPorChave(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave))
            {
                return null;
            }

            var aluno = await _db.Aluno.FirstOrDefaultAsync(a => a.ChaveConfirmacaoEmail == chave);
            if (aluno is not null)
            {
                if (!aluno.EmailConfirmado)
                {
                    aluno.EmailConfirmado = true;
                    await _db.SaveChangesAsync();
                }

                return aluno.TipoParticipante == TipoParticipante.Externo
                    ? new ConfirmacaoEmailResultado("E-mail confirmado com sucesso.", "Publico", ObterPublicLoginUrl())
                    : new ConfirmacaoEmailResultado("E-mail confirmado com sucesso.", "Aluno", ObterMobileLoginDeepLink());
            }

            var usuarioSecretaria = await _db.UsuarioSecretaria.FirstOrDefaultAsync(u => u.Chave == chave);
            if (usuarioSecretaria is not null)
            {
                if (!usuarioSecretaria.EmailConfirmado)
                {
                    usuarioSecretaria.EmailConfirmado = true;
                    await _db.SaveChangesAsync();
                }

                return new ConfirmacaoEmailResultado("E-mail confirmado com sucesso.", "Secretaria", ObterWebLoginUrl());
            }

            var usuarioUnievent = await _db.UsuarioUnievent.FirstOrDefaultAsync(u => u.Chave == chave);
            if (usuarioUnievent is not null)
            {
                usuarioUnievent.EmailConfirmado = true;
                await _db.SaveChangesAsync();
                return new ConfirmacaoEmailResultado("E-mail confirmado com sucesso.", "Admin", ObterWebLoginUrl());
            }

            return null;
        }

        private string ObterPublicLoginUrl() => string.IsNullOrWhiteSpace(_emailSettings.PublicLoginUrl)
            ? $"{_emailSettings.BaseUrl.TrimEnd('/')}/entrar" : _emailSettings.PublicLoginUrl;

        private string ObterWebLoginUrl()
        {
            if (!string.IsNullOrWhiteSpace(_emailSettings.WebLoginUrl))
            {
                return _emailSettings.WebLoginUrl;
            }

            return $"{_emailSettings.BaseUrl.TrimEnd('/')}/login";
        }

        private string ObterMobileLoginDeepLink()
        {
            return _emailSettings.MobileLoginDeepLink;
        }

        private string CriarPaginaRedirecionamentoMobile(string mensagem)
        {
            var mobileLogin = ObterMobileLoginDeepLink();
            var mobileLoginJson = JsonSerializer.Serialize(mobileLogin);
            var webLogin = ObterWebLoginUrl();
            var mensagemHtml = WebUtility.HtmlEncode(mensagem);
            var mobileLoginHtml = WebUtility.HtmlEncode(mobileLogin);
            var webLoginHtml = WebUtility.HtmlEncode(webLogin);

            return $@"<!doctype html>
<html lang=""pt-BR"">
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>UniEvent - E-mail confirmado</title>
  <style>
    body {{ font-family: Arial, sans-serif; margin: 0; padding: 32px; background: #111; color: #fff; }}
    main {{ max-width: 520px; margin: 0 auto; }}
    a {{ color: #ff8a00; font-weight: 700; }}
  </style>
</head>
<body>
  <main>
    <h1>{mensagemHtml}</h1>
    <p>Estamos abrindo o aplicativo UniEvent. Se nada acontecer, abra o app e faça login.</p>
    <p><a href=""{mobileLoginHtml}"">Abrir UniEvent Mobile</a></p>
    <p>Se estiver em um computador, volte para o aplicativo no celular.</p>
    <p><a href=""{webLoginHtml}"">Ir para o login web</a></p>
  </main>
  <script>
    const mobileLogin = {mobileLoginJson};
    if (mobileLogin) {{
      window.location.href = mobileLogin;
    }}
  </script>
</body>
</html>";
        }

        private record ConfirmacaoEmailResultado(string Mensagem, string TipoUsuario, string Destino);
    }
}
