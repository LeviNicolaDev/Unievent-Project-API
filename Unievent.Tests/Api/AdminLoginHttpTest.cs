using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;
using Unievent.Tests.Infrastructure;
using Xunit;

namespace Unievent.Tests.Api;

public class AdminLoginHttpTest
{
    private sealed class App(CertificacaoDatabase database, string environment, bool missingIssuer = false)
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environment);
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "isolated-admin-login-test-key-at-least-32-characters",
                ["Jwt:Issuer"] = missingIssuer ? null : "AdminLoginTests",
                ["Jwt:Audience"] = "AdminLoginTests",
                ["Jwt:ExpirationMinutes"] = "50",
                ["Database:MigrateOnStartup"] = "false",
                ["DemoSeed:Enabled"] = "false",
                ["Automacoes:Ativas"] = "false",
            }));
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDbContextFactory<AppDbContext>>();
                services.RemoveAll<AppDbContext>();
                services.AddSingleton<IDbContextFactory<AppDbContext>>(database);
                services.AddScoped(_ => database.CreateDbContext());
                services.RemoveAll<IEmailService>();
                services.AddSingleton(Mock.Of<IEmailService>());
            });
        }
    }

    private static async Task SeedAdmin(CertificacaoDatabase database)
    {
        await database.SeedAsync();
        await using var db = database.CreateDbContext();
        db.UsuarioUnievent.Add(new UsuarioUnievent
        {
            NomeUsuario = "Admin de teste", EmailUsuario = "admin-login@fatec.sp.gov.br",
            Senha = BCrypt.Net.BCrypt.HashPassword("senha-de-teste"),
            EmailConfirmado = true, IsAtivo = true, Chave = "admin-login-test",
        });
        await db.SaveChangesAsync();
    }

    [Theory]
    [InlineData("Testing")]
    [InlineData("Development")]
    public async Task AdminLogin_ReturnsPlainTextToken_ThatAuthorizesDashboard(string environment)
    {
        await using var database = new CertificacaoDatabase();
        await SeedAdmin(database);
        await using var app = new App(database, environment);
        using var client = app.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        var login = await client.PostAsJsonAsync("/api/Auth/login", new
        {
            email = "admin-login@fatec.sp.gov.br", senha = "senha-de-teste"
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal("text/plain", login.Content.Headers.ContentType?.MediaType);
        var token = await login.Content.ReadAsStringAsync();
        Assert.Equal(3, token.Split('.').Length);

        var unauthenticated = await client.GetAsync("/api/admin/dashboard");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticated.StatusCode);
        Assert.Equal("Bearer", Assert.Single(unauthenticated.Headers.WwwAuthenticate).Scheme);
        Assert.Empty(await unauthenticated.Content.ReadAsStringAsync());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var dashboard = await client.GetAsync("/api/admin/dashboard");
        Assert.Equal(HttpStatusCode.OK, dashboard.StatusCode);
    }

    [Fact]
    public async Task MissingIssuer_CanReturnLoginSuccess_ButDashboardRejectsToken()
    {
        await using var database = new CertificacaoDatabase();
        await SeedAdmin(database);
        await using var app = new App(database, "Testing", missingIssuer: true);
        using var client = app.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        var login = await client.PostAsJsonAsync("/api/Auth/login", new
        {
            email = "admin-login@fatec.sp.gov.br", senha = "senha-de-teste"
        });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await login.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/admin/dashboard")).StatusCode);
    }
}
