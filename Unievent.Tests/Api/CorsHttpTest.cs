using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Unievent.Tests.Api;

public class CorsHttpTest
{
    private sealed class App(string environment) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environment);
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "cors-test-key-with-at-least-32-characters",
                ["Database:MigrateOnStartup"] = "false",
                ["DemoSeed:Enabled"] = "false",
                ["Automacoes:Ativas"] = "false",
                ["Cors:AllowedOrigins:0"] = "https://mobile.example.com"
            }));
        }
    }

    [Theory]
    [InlineData("Development", "http://localhost:8081", true)]
    [InlineData("Development", "http://127.0.0.1:8081", true)]
    [InlineData("Development", "http://192.168.1.20:8081", true)]
    [InlineData("Development", "http://10.0.0.20:19006", true)]
    [InlineData("Development", "http://172.16.0.20:8081", true)]
    [InlineData("Development", "http://192.168.1.20:9999", false)]
    [InlineData("Development", "http://172.32.0.20:8081", false)]
    [InlineData("Development", "http://example.com:8081", false)]
    [InlineData("Production", "http://192.168.1.20:8081", false)]
    [InlineData("Production", "https://mobile.example.com", true)]
    public async Task Preflight_Respeita_Origens_E_Ambiente(string environment, string origin, bool allowed)
    {
        using var app = new App(environment);
        using var client = app.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/Aluno/me");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "authorization,content-type");
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(allowed, response.Headers.Contains("Access-Control-Allow-Origin"));
        if (allowed)
        {
            Assert.Equal(origin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
            Assert.Contains("authorization", string.Join(",", response.Headers.GetValues("Access-Control-Allow-Headers")), StringComparison.OrdinalIgnoreCase);
        }
    }
}
