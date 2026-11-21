using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Unievent.Application.Common;
using Unievent.Application.Configurations.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Infra.Services;

public class PasswordRecoveryService(AppDbContext db, IEmailService emailService,
    IOptions<EmailSettings> settings, ILogger<PasswordRecoveryService> logger)
{
    public const string MensagemSolicitacao = "Se houver uma conta com esse e-mail, enviaremos um link para redefinir sua senha.";
    private const string TokenInvalido = "Link de recuperação inválido ou expirado. Solicite um novo link.";
    private record Conta(int Id, string Tipo, string Nome, string Email, string Senha, string Perfil);

    private async Task<Conta?> BuscarConta(string email, string perfil)
    {
        email = email.Trim().ToLowerInvariant();
        if (perfil == "administrativo")
        {
            var admin = await db.UsuarioUnievent.AsNoTracking().FirstOrDefaultAsync(u => u.IsAtivo && u.EmailUsuario.ToLower() == email);
            if (admin is not null) return new(admin.Id, "Admin", admin.NomeUsuario, admin.EmailUsuario, admin.Senha, "Admin");
            var secretaria = await db.UsuarioSecretaria.AsNoTracking().FirstOrDefaultAsync(u => u.IsAtivo && u.EmailUsuario.ToLower() == email);
            return secretaria is null ? null : new(secretaria.Id, "Secretaria", secretaria.NomeUsuario, secretaria.EmailUsuario, secretaria.Senha, "Secretaria");
        }
        if (perfil is not ("publico" or "mobile")) return null;
        var tipo = perfil == "publico" ? TipoParticipante.Externo : TipoParticipante.Interno;
        var aluno = await db.Aluno.AsNoTracking().FirstOrDefaultAsync(a => a.IsAtivo && a.Email.ToLower() == email && a.TipoParticipante == tipo);
        return aluno is null ? null : new(aluno.Id, "Aluno", aluno.Nome, aluno.Email, aluno.Senha, perfil == "publico" ? "Publico" : "Aluno");
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    public async Task<bool> SessaoValidaAsync(int id, string tipoConta, long? emitidoEmTicks)
    {
        var ultimaTroca = await db.RecuperacaoSenha.Where(r => r.UsuarioId == id && r.TipoConta == tipoConta)
            .MaxAsync(r => r.UsadoEmUtc);
        return ultimaTroca is null || (emitidoEmTicks.HasValue && emitidoEmTicks.Value > ultimaTroca.Value.Ticks);
    }

    public async Task SolicitarAsync(string email, string perfil)
    {
        var conta = await BuscarConta(email, perfil);
        if (conta is null) return;
        var now = DateTime.UtcNow;
        if (await db.RecuperacaoSenha.AnyAsync(r => r.TipoConta == conta.Tipo && r.UsuarioId == conta.Id && r.CriadoEmUtc > now.AddMinutes(-1))) return;
        if (!Uri.TryCreate(settings.Value.BaseUrl, UriKind.Absolute, out var origin) ||
            (origin.Scheme != "https" && !(origin.Scheme == "http" && origin.IsLoopback)))
        {
            logger.LogError("Configure EmailSettings:BaseUrl com a origem HTTPS pública do front para recuperação de senha.");
            return;
        }
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var registro = new RecuperacaoSenha {
            TokenHash = Hash(token), TipoConta = conta.Tipo, UsuarioId = conta.Id,
            SenhaFingerprint = Hash(conta.Senha), CriadoEmUtc = now, ExpiraEmUtc = now.AddMinutes(30)
        };
        db.RecuperacaoSenha.Add(registro);
        await db.SaveChangesAsync();
        var link = $"{origin.GetLeftPart(UriPartial.Authority)}/redefinir-senha?token={Uri.EscapeDataString(token)}&perfil={Uri.EscapeDataString(perfil)}";
        try
        {
            var result = await emailService.SendAsync(conta.Email, "Redefinição de senha - UniEvent",
                $"<h1>Olá, {WebUtility.HtmlEncode(conta.Nome)}!</h1><p>Recebemos uma solicitação de redefinição de senha.</p>" +
                $"<p><a href=\"{WebUtility.HtmlEncode(link)}\">Redefinir minha senha</a></p><p>O link expira em 30 minutos e pode ser usado uma única vez.</p>" +
                "<p>Se você não fez essa solicitação, ignore esta mensagem. Sua senha continua a mesma.</p>");
            if (result.IsSuccess) return;
            logger.LogWarning("SMTP recusou o envio de recuperação de senha.");
        }
        catch (Exception ex) { logger.LogWarning("Falha de recuperação SMTP: {Tipo}", ex.GetType().Name); }
        db.RecuperacaoSenha.Remove(registro);
        await db.SaveChangesAsync();
    }

    public async Task<Result<string>> RedefinirAsync(string token, string senha, string confirmacao)
    {
        if (senha.Length < 8 || Encoding.UTF8.GetByteCount(senha) > 72 || senha != confirmacao)
            return Result<string>.Failure("Informe senhas iguais, com no mínimo 8 caracteres e no máximo 72 bytes.");
        if (token.Length != 64 || !token.All(Uri.IsHexDigit)) return Result<string>.Failure(TokenInvalido);
        var hash = Hash(token);
        var registro = await db.RecuperacaoSenha.AsNoTracking().SingleOrDefaultAsync(r => r.TokenHash == hash);
        var now = DateTime.UtcNow;
        if (registro is null || registro.UsadoEmUtc.HasValue || registro.ExpiraEmUtc <= now) return Result<string>.Failure(TokenInvalido);

        Conta? conta = null;
        if (registro.TipoConta == "Admin")
        {
            var u = await db.UsuarioUnievent.AsNoTracking().FirstOrDefaultAsync(u => u.Id == registro.UsuarioId && u.IsAtivo);
            if (u is not null) conta = new(u.Id, "Admin", u.NomeUsuario, u.EmailUsuario, u.Senha, "Admin");
        }
        else if (registro.TipoConta == "Secretaria")
        {
            var u = await db.UsuarioSecretaria.AsNoTracking().FirstOrDefaultAsync(u => u.Id == registro.UsuarioId && u.IsAtivo);
            if (u is not null) conta = new(u.Id, "Secretaria", u.NomeUsuario, u.EmailUsuario, u.Senha, "Secretaria");
        }
        else if (registro.TipoConta == "Aluno")
        {
            var u = await db.Aluno.AsNoTracking().FirstOrDefaultAsync(u => u.Id == registro.UsuarioId && u.IsAtivo);
            if (u is not null) conta = new(u.Id, "Aluno", u.Nome, u.Email, u.Senha, u.TipoParticipante == TipoParticipante.Externo ? "Publico" : "Aluno");
        }
        if (conta is null || Hash(conta.Senha) != registro.SenhaFingerprint) return Result<string>.Failure(TokenInvalido);
        var novaSenha = BCrypt.Net.BCrypt.HashPassword(senha);
        await using var transaction = await db.Database.BeginTransactionAsync();
        // A atualização condicional serializa redefinições concorrentes da mesma conta.
        var updated = conta.Tipo switch {
            "Admin" => await db.UsuarioUnievent.Where(u => u.Id == conta.Id && u.Senha == conta.Senha && u.IsAtivo).ExecuteUpdateAsync(s => s.SetProperty(u => u.Senha, novaSenha)),
            "Secretaria" => await db.UsuarioSecretaria.Where(u => u.Id == conta.Id && u.Senha == conta.Senha && u.IsAtivo).ExecuteUpdateAsync(s => s.SetProperty(u => u.Senha, novaSenha)),
            _ => await db.Aluno.Where(u => u.Id == conta.Id && u.Senha == conta.Senha && u.IsAtivo).ExecuteUpdateAsync(s => s.SetProperty(u => u.Senha, novaSenha))
        };
        if (updated != 1) return Result<string>.Failure(TokenInvalido);
        var consumedAt = DateTime.UtcNow;
        var consumed = await db.RecuperacaoSenha.Where(r => r.Id == registro.Id && r.UsadoEmUtc == null && r.ExpiraEmUtc > consumedAt)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.UsadoEmUtc, (DateTime?)consumedAt));
        if (consumed != 1) return Result<string>.Failure(TokenInvalido);
        await db.RecuperacaoSenha.Where(r => r.TipoConta == conta.Tipo && r.UsuarioId == conta.Id && r.UsadoEmUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.UsadoEmUtc, (DateTime?)consumedAt));
        await transaction.CommitAsync();
        return Result<string>.Success(conta.Perfil);
    }
}
