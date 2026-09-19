using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Tests.Infrastructure;

public sealed class CertificacaoDatabase : IDbContextFactory<AppDbContext>, IAsyncDisposable
{
    private readonly string path = Path.Combine(Path.GetTempPath(), $"unievent-certificados-{Guid.NewGuid():N}.db");
    public DbContextOptions<AppDbContext> Options { get; }
    public CertificacaoDatabase()
    {
        var sqlServer = Environment.GetEnvironmentVariable("UNIEVENT_TEST_SQLSERVER");
        Options = string.IsNullOrEmpty(sqlServer)
            ? new DbContextOptionsBuilder<AppDbContext>().UseSqlite($"Data Source={path}").Options
            : new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(new SqlConnectionStringBuilder(sqlServer)
            {
                InitialCatalog = $"UnieventCertificacaoTest_{Guid.NewGuid():N}"
            }.ConnectionString).Options;
    }
    public AppDbContext CreateDbContext() => new(Options);
    public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) => Task.FromResult(CreateDbContext());

    public async Task<Participacao> SeedAsync(bool certificado = true, bool presente = true,
        TipoParticipante tipo = TipoParticipante.Interno)
    {
        await using var db = CreateDbContext();
        await db.Database.EnsureCreatedAsync();
        var instituicao = new Instituicao
        {
            Nome = "FATEC Ferraz de Vasconcelos", FotoPerfil = "", Cnpj = "12345678000190",
            Rua = "Rua A", Cidade = "Ferraz", Bairro = "Centro", Estado = "SP", Cep = "08500000", Numero = "1"
        };
        var evento = new Evento
        {
            Nome = "Semana de Tecnologia e Inovação", Descricao = "Evento de teste",
            Categoria = Categoria.Palestra,
            DataEvento = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo")),
            Capacidade = 10, Thumbnail = [],
            ResponsavelEventoId = 1, InstituicaoId = 1, Instituicao = instituicao,
            ResponsavelEvento = new ResponsavelEvento { Nome = "Professora Érica", FotoPerfil = "", Instituicao = instituicao }
        };
        var aluno = new Aluno
        {
            Nome = "João Gonçalves da Silva", Email = tipo == TipoParticipante.Interno ? "joao@fatec.sp.gov.br" : "joao@example.com",
            Senha = "hash-de-teste", FotoPerfil = "", IsAtivo = true, EmailConfirmado = true,
            DataNascimento = new DateTime(2000, 1, 1), Role = Role.Aluno, TipoParticipante = tipo,
            Instituicao = tipo == TipoParticipante.Interno ? instituicao : null
        };
        var participacao = new Participacao(1, 1) { Aluno = aluno, Evento = evento };
        if (presente) participacao.ConfirmarPresencaPorCodigo(10);
        db.Participacao.Add(participacao);
        if (certificado) db.Certificado.Add(new Certificado
        {
            Evento = evento, EventoId = 1, DataCertifcado = DateTime.UtcNow, Texto = "Participação nas atividades. Carga horária: 4 horas."
        });
        db.UsuarioSecretaria.Add(new UsuarioSecretaria
        {
            NomeUsuario = "Secretaria", EmailUsuario = "secretaria@fatec.sp.gov.br", Senha = "hash",
            Chave = "chave-de-teste", RoleUsuario = Role.Secretaria, Instituicao = instituicao, IsAtivo = true,
            EmailConfirmado = true, Status = StatusUsuarioSecretaria.Ativo
        });
        await db.SaveChangesAsync();
        return participacao;
    }

    public async Task<Participacao> ParticipacaoAsync()
    {
        await using var db = CreateDbContext();
        return await db.Participacao.AsNoTracking().SingleAsync(p => p.Id == 1);
    }

    public async ValueTask DisposeAsync()
    {
        await using var db = CreateDbContext();
        await db.Database.EnsureDeletedAsync();
    }
}
