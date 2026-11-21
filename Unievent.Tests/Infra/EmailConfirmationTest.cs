using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Unievent.Api.Controllers;
using Unievent.Application.Common;
using Unievent.Application.Configurations.Email;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;
using Xunit;

namespace Unievent.Tests.Infra;

public class EmailConfirmationTest : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly Mock<IEmailService> email = new();
    private AppDbContext db = null!;
    private EmailController controller = null!;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        await db.Database.EnsureCreatedAsync();
        controller = new EmailController(email.Object, db, Options.Create(new EmailSettings {
            BaseUrl = "https://web.example.com", WebLoginUrl = "https://web.example.com/login",
            MobileLoginDeepLink = "unievent://login"
        }));
        email.Setup(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<bool>.Success(true));
    }

    public async Task DisposeAsync() { await db.DisposeAsync(); await connection.DisposeAsync(); }

    [Fact]
    public async Task Admin_Confirma_Persiste_E_Permite_Reabrir_Link()
    {
        var admin = new UsuarioUnievent { NomeUsuario = "Admin", EmailUsuario = "admin@example.com", Senha = "hash", Chave = "admin-key" };
        db.UsuarioUnievent.Add(admin);
        await db.SaveChangesAsync();
        var result = await controller.ConfirmarContaPorLinkAsync("admin-key");
        result.Should().BeOfType<RedirectResult>().Which.Url.Should().Be("https://web.example.com/login");
        db.ChangeTracker.Clear();
        (await db.UsuarioUnievent.SingleAsync()).EmailConfirmado.Should().BeTrue();
        (await controller.ConfirmarContaAsync(new ConfirmarContaRequest { Chave = "admin-key" })).Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData(TipoParticipante.Externo)]
    [InlineData(TipoParticipante.Interno)]
    public async Task Participante_Confirma_No_Destino_Do_Perfil(TipoParticipante tipo)
    {
        db.Aluno.Add(new Aluno { Nome = "Pessoa", Email = "pessoa@example.com", Senha = "hash", FotoPerfil = "",
            IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1), TipoParticipante = tipo, ChaveConfirmacaoEmail = "participant-key" });
        await db.SaveChangesAsync();
        var result = await controller.ConfirmarContaPorLinkAsync("participant-key");
        if (tipo == TipoParticipante.Externo)
            result.Should().BeOfType<RedirectResult>().Which.Url.Should().Be("https://web.example.com/entrar");
        else
            result.Should().BeOfType<ContentResult>().Which.Content.Should().Contain("unievent://login");
        (await db.Aluno.SingleAsync()).EmailConfirmado.Should().BeTrue();
    }

    [Fact]
    public async Task Chave_Invalida_Nao_Ativa_Conta()
    {
        (await controller.ConfirmarContaAsync(new ConfirmarContaRequest { Chave = "invalid" })).Should().BeOfType<NotFoundObjectResult>();
        (await controller.ConfirmarContaAsync(new ConfirmarContaRequest { Chave = " " })).Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Reenvio_Gera_Chave_No_Servidor_E_Nao_Confirma_Conta()
    {
        var admin = new UsuarioUnievent { NomeUsuario = "Admin", EmailUsuario = "admin@example.com", Senha = "hash", Chave = "old-key" };
        db.UsuarioUnievent.Add(admin);
        await db.SaveChangesAsync();
        (await controller.ReenviarConfirmacao(new("admin@example.com"))).Should().BeOfType<OkObjectResult>();
        admin.Chave.Should().NotBe("old-key").And.HaveLength(32);
        admin.EmailConfirmado.Should().BeFalse();
        email.Verify(e => e.EnviarEmailConfirmacaoConta(admin.EmailUsuario, admin.NomeUsuario, admin.Chave), Times.Once);
        (await controller.ConfirmarContaAsync(new ConfirmarContaRequest { Chave = "old-key" })).Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Falha_Reenvio_Preserva_Chave_Anterior()
    {
        var admin = new UsuarioUnievent { NomeUsuario = "Admin", EmailUsuario = "admin@example.com", Senha = "hash", Chave = "old-key" };
        db.UsuarioUnievent.Add(admin);
        await db.SaveChangesAsync();
        email.Setup(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<bool>.Failure("SMTP indisponível"));
        (await controller.ReenviarConfirmacao(new("admin@example.com"))).Should().BeOfType<BadRequestObjectResult>();
        admin.Chave.Should().Be("old-key");
    }
}
