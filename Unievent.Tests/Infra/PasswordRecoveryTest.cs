using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Unievent.Application.Common;
using Unievent.Application.Configurations.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;
using Unievent.Infra.Services;
using Xunit;

namespace Unievent.Tests.Infra;

public class PasswordRecoveryTest : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly Mock<IEmailService> email = new();
    private AppDbContext db = null!;
    private PasswordRecoveryService service = null!;
    private string? html;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        db = new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        await db.Database.EnsureCreatedAsync();
        email.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string, string>((_, _, body) => html = body).ReturnsAsync(Result<bool>.Success(true));
        service = new(db, email.Object, Options.Create(new EmailSettings { BaseUrl = "https://unievent-web.vercel.app" }), Mock.Of<ILogger<PasswordRecoveryService>>());
    }
    public async Task DisposeAsync() { await db.DisposeAsync(); await connection.DisposeAsync(); }
    private string Token() => Regex.Match(html!, "token=([A-F0-9]+)").Groups[1].Value;
    private async Task Seed(string tipo)
    {
        var senha = BCrypt.Net.BCrypt.HashPassword("antiga123");
        if (tipo == "admin") db.UsuarioUnievent.Add(new() { NomeUsuario = "Admin", EmailUsuario = "pessoa@example.com", Senha = senha });
        else if (tipo == "secretaria") db.UsuarioSecretaria.Add(new() { NomeUsuario = "Secretaria", EmailUsuario = "pessoa@example.com", Senha = senha, RoleUsuario = Role.Secretaria, IsAtivo = true });
        else db.Aluno.Add(new() { Nome = "Pessoa", Email = "pessoa@example.com", Senha = senha, FotoPerfil = "", DataNascimento = new DateTime(2000, 1, 1), IsAtivo = true,
            TipoParticipante = tipo == "mobile" ? TipoParticipante.Interno : TipoParticipante.Externo });
        await db.SaveChangesAsync();
    }
    private static string Perfil(string tipo) => tipo is "admin" or "secretaria" ? "administrativo" : tipo;

    [Theory]
    [InlineData("admin", "Admin")]
    [InlineData("secretaria", "Secretaria")]
    [InlineData("publico", "Publico")]
    [InlineData("mobile", "Aluno")]
    public async Task Redefine_Quatro_Perfis_Sem_Ativar_Email_E_Com_Uso_Unico(string tipo, string esperado)
    {
        await Seed(tipo);
        await service.SolicitarAsync(" PESSOA@example.com ", Perfil(tipo));
        html.Should().Contain("https://unievent-web.vercel.app/redefinir-senha?token=");
        html.Should().Contain($"perfil={Perfil(tipo)}");
        var token = Token();
        token.Should().HaveLength(64);
        var record = await db.RecuperacaoSenha.AsNoTracking().SingleAsync();
        record.TokenHash.Should().NotBe(token);
        var result = await service.RedefinirAsync(token, "abc123", "abc123");
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(esperado);
        (await service.SessaoValidaAsync(1, record.TipoConta, null)).Should().BeFalse();
        (await service.SessaoValidaAsync(1, record.TipoConta, DateTime.UtcNow.Ticks)).Should().BeTrue();
        var senha = tipo == "admin" ? (await db.UsuarioUnievent.AsNoTracking().SingleAsync()).Senha
            : tipo == "secretaria" ? (await db.UsuarioSecretaria.AsNoTracking().SingleAsync()).Senha
            : (await db.Aluno.AsNoTracking().SingleAsync()).Senha;
        BCrypt.Net.BCrypt.Verify("abc123", senha).Should().BeTrue();
        BCrypt.Net.BCrypt.Verify("antiga123", senha).Should().BeFalse();
        var confirmado = tipo == "admin" ? (await db.UsuarioUnievent.AsNoTracking().SingleAsync()).EmailConfirmado
            : tipo == "secretaria" ? (await db.UsuarioSecretaria.AsNoTracking().SingleAsync()).EmailConfirmado
            : (await db.Aluno.AsNoTracking().SingleAsync()).EmailConfirmado;
        confirmado.Should().BeFalse();
        (await service.RedefinirAsync(token, "outraSenha123", "outraSenha123")).IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Email_Inexistente_Nao_Gera_Token_Nem_Envia_Email()
    {
        await service.SolicitarAsync("ausente@example.com", "publico");
        (await db.RecuperacaoSenha.CountAsync()).Should().Be(0);
        email.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    [Fact]
    public async Task Token_Expirado_Nao_Altera_Senha()
    {
        await Seed("publico");
        await service.SolicitarAsync("pessoa@example.com", "publico");
        await db.RecuperacaoSenha.ExecuteUpdateAsync(s => s.SetProperty(r => r.ExpiraEmUtc, DateTime.UtcNow.AddMinutes(-1)));
        (await service.RedefinirAsync(Token(), "abc123", "abc123")).IsFailure.Should().BeTrue();
        BCrypt.Net.BCrypt.Verify("antiga123", (await db.Aluno.AsNoTracking().SingleAsync()).Senha).Should().BeTrue();
    }
    [Fact]
    public async Task Nao_Envia_Repetidamente_No_Mesmo_Minuto()
    {
        await Seed("publico");
        await service.SolicitarAsync("pessoa@example.com", "publico");
        await service.SolicitarAsync("pessoa@example.com", "publico");
        email.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    [Theory]
    [InlineData("curta", "curta")]
    [InlineData("abc123", "diferente123")]
    public async Task Senha_Invalida_Nao_Consome_Token(string senha, string confirmacao)
    {
        await Seed("publico");
        await service.SolicitarAsync("pessoa@example.com", "publico");
        (await service.RedefinirAsync(Token(), senha, confirmacao)).IsFailure.Should().BeTrue();
        (await db.RecuperacaoSenha.AsNoTracking().SingleAsync()).UsadoEmUtc.Should().BeNull();
    }
    [Fact]
    public async Task Falha_Smtp_Remove_Token_Para_Permitir_Nova_Tentativa()
    {
        await Seed("publico");
        email.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Result<bool>.Failure("Falha"));
        await service.SolicitarAsync("pessoa@example.com", "publico");
        (await db.RecuperacaoSenha.CountAsync()).Should().Be(0);
    }
    [Fact]
    public async Task Troca_De_Senha_Fora_Do_Fluxo_Invalida_Token()
    {
        await Seed("publico");
        await service.SolicitarAsync("pessoa@example.com", "publico");
        var hash = BCrypt.Net.BCrypt.HashPassword("outraSenha123");
        await db.Aluno.ExecuteUpdateAsync(s => s.SetProperty(a => a.Senha, hash));
        (await service.RedefinirAsync(Token(), "abc123", "abc123")).IsFailure.Should().BeTrue();
    }
    [Fact]
    public async Task Redefinir_Invalida_Outros_Links_Da_Mesma_Conta()
    {
        await Seed("publico");
        await service.SolicitarAsync("pessoa@example.com", "publico");
        var primeiro = Token();
        await db.RecuperacaoSenha.ExecuteUpdateAsync(s => s.SetProperty(r => r.CriadoEmUtc, DateTime.UtcNow.AddMinutes(-2)));
        await service.SolicitarAsync("pessoa@example.com", "publico");
        var segundo = Token();
        (await service.RedefinirAsync(primeiro, "abc123", "abc123")).IsSuccess.Should().BeTrue();
        (await service.RedefinirAsync(segundo, "outraSenha123", "outraSenha123")).IsFailure.Should().BeTrue();
    }
}
