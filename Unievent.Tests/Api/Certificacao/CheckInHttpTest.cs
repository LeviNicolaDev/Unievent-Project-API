using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Unievent.Application.Dtos.Email;
using Unievent.Application.Interfaces.Auth;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;
using Unievent.Tests.Infrastructure;
using Xunit;

namespace Unievent.Tests.Api.Certificacao;

public class CheckInHttpTest
{
    private sealed class App(CertificacaoDatabase database, IEmailService email) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-local-exclusiva-testes-certificado-2026",
                ["Jwt:Issuer"] = "UnieventTests", ["Jwt:Audience"] = "UnieventTests", ["Jwt:ExpirationMinutes"] = "10",
                ["Database:MigrateOnStartup"] = "false", ["Automacoes:Ativas"] = "false"
            }));
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDbContextFactory<AppDbContext>>();
                services.RemoveAll<AppDbContext>();
                services.AddSingleton<IDbContextFactory<AppDbContext>>(database);
                services.AddScoped(_ => database.CreateDbContext());
                services.RemoveAll<IEmailService>();
                services.AddSingleton(email);
            });
        }

        public HttpClient Client(Role? role, int id = 1, int? instituicao = 1)
        {
            var client = CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
            if (role.HasValue)
            {
                using var scope = Services.CreateScope();
                var jwt = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
                var token = role == Role.Aluno
                    ? jwt.GerarToken(id, "participante@example.com", role.Value, TipoParticipante.Externo, instituicao)
                    : jwt.GerarToken(id, "secretaria@example.com", role.Value, instituicao);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }
    }

    private static Mock<IEmailService> Email(bool falha = false)
    {
        var email = new Mock<IEmailService>();
        email.Setup(e => e.SendWithAttachmentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<EmailAnexo>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EnvioEmailResultado(falha ? SituacaoEnvioEmail.FalhaTemporaria : SituacaoEnvioEmail.Enviado));
        return email;
    }

    [Theory]
    [InlineData(null, 401)]
    [InlineData(Role.Aluno, 403)]
    public async Task Endpoint_Bloqueia_Anonimo_E_Participante(Role? role, int status)
    {
        await using var db = new CertificacaoDatabase();
        var p = await db.SeedAsync(presente: false);
        var email = Email();
        await using var app = new App(db, email.Object);
        using var client = app.Client(role);
        var response = await client.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = p.CodigoIngresso });
        ((int)response.StatusCode).Should().Be(status);
        (await db.ParticipacaoAsync()).PresencaConfirmada.Should().BeFalse();
        email.Invocations.Should().BeEmpty();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(null)]
    public async Task Secretaria_Nao_Pode_Validar_Outra_Instituicao_Nem_Omitir_Vinculo(int? instituicao)
    {
        await using var db = new CertificacaoDatabase();
        var p = await db.SeedAsync(presente: false);
        var email = Email();
        await using var app = new App(db, email.Object);
        using var client = app.Client(Role.Secretaria, instituicao: instituicao);
        var response = await client.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = p.CodigoIngresso, eventoId = 1 });
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await db.ParticipacaoAsync()).PresencaConfirmada.Should().BeFalse();
        email.Invocations.Should().BeEmpty();
    }

    [Theory]
    [InlineData(TipoParticipante.Interno, false)]
    [InlineData(TipoParticipante.Externo, false)]
    [InlineData(TipoParticipante.Externo, true)]
    public async Task CheckIn_Http_Gera_Pdf_Repeticao_Nao_Reenvia_E_Falha_Nao_Desfaz_Presenca(TipoParticipante tipo, bool falha)
    {
        await using var db = new CertificacaoDatabase();
        var p = await db.SeedAsync(presente: false, tipo: tipo);
        var email = Email(falha);
        await using var app = new App(db, email.Object);
        using var client = app.Client(Role.Secretaria);
        for (var i = 0; i < 2; i++)
        {
            var response = await client.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = p.CodigoIngresso });
            response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        }
        var saved = await db.ParticipacaoAsync();
        saved.PresencaConfirmada.Should().BeTrue();
        saved.CertificadoEmitido.Should().BeTrue();
        saved.CertificadoEnviadoPorEmail.Should().Be(!falha);
        saved.CertificadoPdf.Should().NotBeNull();
        email.Invocations.Should().HaveCount(1);
        using var participante = app.Client(Role.Aluno, id: 1, instituicao: null);
        var download = await participante.GetAsync("/api/Certificado/eventos/1/pdf");
        download.StatusCode.Should().Be(HttpStatusCode.OK);
        download.Content.Headers.ContentType!.MediaType.Should().Be("application/pdf");
        (await download.Content.ReadAsByteArrayAsync()).Should().Equal(saved.CertificadoPdf!);
        using var outro = app.Client(Role.Aluno, id: 2, instituicao: null);
        (await outro.GetAsync("/api/Certificado/eventos/1/pdf")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Evento_Sem_Certificado_Confirma_Normalmente_E_Nao_Envia()
    {
        await using var db = new CertificacaoDatabase();
        var p = await db.SeedAsync(certificado: false, presente: false);
        var email = Email();
        await using var app = new App(db, email.Object);
        using var client = app.Client(Role.Secretaria);
        (await client.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = p.CodigoIngresso })).StatusCode.Should().Be(HttpStatusCode.OK);
        (await db.ParticipacaoAsync()).PresencaConfirmada.Should().BeTrue();
        (await db.ParticipacaoAsync()).CertificadoEmitido.Should().BeFalse();
        email.Invocations.Should().BeEmpty();
        // A emissão manual também deve respeitar a existência da configuração.
        (await client.PostAsync("/api/Certificado/eventos/1/alunos/1/emitir", null)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Secretaria_Bloqueada_Apos_Emitir_Jwt_Nao_Confirma_Presenca()
    {
        await using var db = new CertificacaoDatabase();
        var p = await db.SeedAsync(presente: false);
        await using var app = new App(db, Email().Object);
        using var client = app.Client(Role.Secretaria);
        await using (var context = db.CreateDbContext())
            await context.UsuarioSecretaria.ExecuteUpdateAsync(s => s.SetProperty(u => u.Status, StatusUsuarioSecretaria.Bloqueado));
        (await client.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = p.CodigoIngresso })).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Id_De_Evento_Forjado_Nao_Confirma_Presenca()
    {
        await using var db = new CertificacaoDatabase();
        var p = await db.SeedAsync(presente: false);
        await using var app = new App(db, Email().Object);
        using var client = app.Client(Role.Secretaria);
        (await client.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = p.CodigoIngresso, eventoId = 99 })).StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await db.ParticipacaoAsync()).PresencaConfirmada.Should().BeFalse();
    }

    [Fact]
    public async Task Meus_Ingressos_Exige_Login_E_Usa_Identidade_Do_Jwt()
    {
        await using var db = new CertificacaoDatabase();
        await db.SeedAsync(certificado: false, presente: false, tipo: TipoParticipante.Externo);
        await using var app = new App(db, Email().Object);

        using var anonimo = app.Client(null);
        (await anonimo.GetAsync("/api/Evento/meus-ingressos")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        using var titular = app.Client(Role.Aluno, id: 1, instituicao: null);
        using var respostaTitular = await titular.GetAsync("/api/Evento/meus-ingressos");
        respostaTitular.StatusCode.Should().Be(HttpStatusCode.OK);
        using var jsonTitular = JsonDocument.Parse(await respostaTitular.Content.ReadAsStringAsync());
        jsonTitular.RootElement.GetArrayLength().Should().Be(1);
        jsonTitular.RootElement[0].GetProperty("nomeEvento").GetString().Should().Be("Semana de Tecnologia e Inovação");

        using var outroParticipante = app.Client(Role.Aluno, id: 2, instituicao: null);
        using var respostaOutro = await outroParticipante.GetAsync("/api/Evento/meus-ingressos");
        respostaOutro.StatusCode.Should().Be(HttpStatusCode.OK);
        using var jsonOutro = JsonDocument.Parse(await respostaOutro.Content.ReadAsStringAsync());
        jsonOutro.RootElement.GetArrayLength().Should().Be(0);
        (await outroParticipante.GetAsync("/api/Evento/1/ingresso")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Duas_Inscricoes_Geram_Ingressos_E_Qr_Codes_Distintos_Para_O_Evento_Correto()
    {
        await using var db = new CertificacaoDatabase();
        var primeiro = await db.SeedAsync(certificado: false, presente: false, tipo: TipoParticipante.Externo);
        await using (var context = db.CreateDbContext())
        {
            var evento = new Evento
            {
                Nome = "Segundo evento", Descricao = "Outro evento", Categoria = Categoria.Workshop,
                DataEvento = DateTime.UtcNow.AddDays(1), Capacidade = 20, Thumbnail = [],
                ResponsavelEventoId = 1, InstituicaoId = 1
            };
            context.Evento.Add(evento);
            await context.SaveChangesAsync();
            context.Participacao.Add(new Participacao(1, evento.Id));
            await context.SaveChangesAsync();
        }

        await using var app = new App(db, Email().Object);
        using var client = app.Client(Role.Aluno, id: 1, instituicao: null);
        using var response = await client.GetAsync("/api/Evento/meus-ingressos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var tickets = json.RootElement.EnumerateArray().ToList();

        tickets.Should().HaveCount(2);
        tickets.Select(t => t.GetProperty("eventoId").GetInt32()).Should().BeEquivalentTo([1, 2]);
        var codes = tickets.Select(t => t.GetProperty("codigoIngresso").GetString()).ToList();
        codes.Should().OnlyHaveUniqueItems();
        codes.Should().Contain(primeiro.CodigoIngresso);
        tickets.Should().OnlyContain(t =>
            t.GetProperty("conteudoQrCode").GetString() == t.GetProperty("codigoIngresso").GetString());
    }

    [Fact]
    public async Task Qr_Code_Atualiza_Status_Do_Ingresso_Apos_CheckIn_E_Repeticao_E_Idempotente()
    {
        await using var db = new CertificacaoDatabase();
        var participacao = await db.SeedAsync(certificado: false, presente: false, tipo: TipoParticipante.Externo);
        await using var app = new App(db, Email().Object);
        using var participante = app.Client(Role.Aluno, id: 1, instituicao: null);

        using (var antes = await participante.GetAsync("/api/Evento/1/ingresso"))
        {
            using var jsonAntes = JsonDocument.Parse(await antes.Content.ReadAsStringAsync());
            jsonAntes.RootElement.GetProperty("conteudoQrCode").GetString().Should().Be(participacao.CodigoIngresso);
            jsonAntes.RootElement.GetProperty("presencaConfirmada").GetBoolean().Should().BeFalse();
        }

        using var secretaria = app.Client(Role.Secretaria);
        for (var i = 0; i < 2; i++)
        {
            var checkIn = await secretaria.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = participacao.CodigoIngresso });
            checkIn.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        using var depois = await participante.GetAsync("/api/Evento/1/ingresso");
        using var jsonDepois = JsonDocument.Parse(await depois.Content.ReadAsStringAsync());
        jsonDepois.RootElement.GetProperty("presencaConfirmada").GetBoolean().Should().BeTrue();
        jsonDepois.RootElement.GetProperty("podeRealizarCheckIn").GetBoolean().Should().BeFalse();
        (await db.ParticipacaoAsync()).PresencaConfirmada.Should().BeTrue();
    }

    [Fact]
    public async Task Qr_Code_Invalido_Ou_De_Inscricao_Cancelada_E_Rejeitado()
    {
        await using var db = new CertificacaoDatabase();
        var participacao = await db.SeedAsync(certificado: false, presente: false, tipo: TipoParticipante.Externo);
        await using var app = new App(db, Email().Object);
        using var secretaria = app.Client(Role.Secretaria);

        var invalido = await secretaria.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = "codigo-inexistente" });
        invalido.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using (var context = db.CreateDbContext())
        {
            var inscricao = await context.Participacao.SingleAsync();
            inscricao.CancelarInscricao();
            await context.SaveChangesAsync();
        }

        var cancelado = await secretaria.PostAsJsonAsync("/api/Evento/check-in", new { codigoIngresso = participacao.CodigoIngresso });
        cancelado.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await cancelado.Content.ReadAsStringAsync()).Should().Contain("cancelada");
        (await db.ParticipacaoAsync()).PresencaConfirmada.Should().BeFalse();

        using var participante = app.Client(Role.Aluno, id: 1, instituicao: null);
        using var ingresso = await participante.GetAsync("/api/Evento/1/ingresso");
        using var json = JsonDocument.Parse(await ingresso.Content.ReadAsStringAsync());
        json.RootElement.GetProperty("statusInscricao").GetString().Should().Be("cancelada");
        json.RootElement.GetProperty("podeRealizarCheckIn").GetBoolean().Should().BeFalse();
    }
}
