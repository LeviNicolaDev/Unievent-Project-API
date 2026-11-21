using Microsoft.EntityFrameworkCore;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Deployment;

public static class DemoDataSeed
{
    public const string SeedId = "demo-eventos-v1";
    public const string ImagePath = "/imagens/school-placeholder.png";

    public static async Task RunAsync(AppDbContext db, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (!db.Database.IsNpgsql())
            throw new InvalidOperationException("A carga demonstrativa requer PostgreSQL.");

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        // All replicas use the same transaction-scoped lock, before checking the marker.
        await db.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock(741902613);", cancellationToken);
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "__DeploymentSeeds" (
                "Id" text PRIMARY KEY,
                "AppliedAtUtc" timestamp with time zone NOT NULL
            );
            """, cancellationToken);
        var applied = await db.Database.SqlQuery<int>(
            $"SELECT COUNT(*)::integer AS \"Value\" FROM \"__DeploymentSeeds\" WHERE \"Id\" = {SeedId}")
            .SingleAsync(cancellationToken);
        if (applied > 0)
        {
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("Carga {SeedId} já aplicada; nenhum registro alterado.", SeedId);
            return;
        }

        var institutions = await db.Instituicao.OrderBy(x => x.Id).Take(3).ToListAsync(cancellationToken);
        if (institutions.Count == 0)
        {
            var institution = new Instituicao
            {
                Nome = "Instituição de demonstração UniEvent", NomeAbreviado = "UniEvent Demo",
                Codigo = "UNIEVENT-DEMO-V1", FotoPerfil = ImagePath, Cnpj = "00000000000000",
                Rua = "Rua de demonstração", Numero = "0", Bairro = "Centro",
                Cidade = "São Paulo", Estado = "SP", Cep = "01001000"
            };
            db.Instituicao.Add(institution);
            institutions.Add(institution);
        }
        var names = new[] { "Ana Silva (demonstração)", "Bruno Oliveira (demonstração)", "Carla Santos (demonstração)" };
        var owners = names.Select((name, index) => new ResponsavelEvento
        {
            Nome = name, FotoPerfil = ImagePath, Instituicao = institutions[index % institutions.Count]
        }).ToArray();
        db.ResponsavelEvento.AddRange(owners);
        await db.SaveChangesAsync(cancellationToken);
        var titles = new[] { "Introdução à inteligência artificial", "Workshop de desenvolvimento web",
            "Feira de projetos acadêmicos", "Carreira e oportunidades em tecnologia",
            "Oficina de análise de dados", "Encontro de inovação e empreendedorismo" };
        var categories = new[] { Categoria.Palestra, Categoria.Workshop, Categoria.Feira,
            Categoria.Palestra, Categoria.Workshop, Categoria.Evento };
        var now = DateTime.UtcNow;
        for (var i = 0; i < titles.Length; i++)
        {
            var owner = owners[i % owners.Length];
            var date = now.Date.AddDays(7 + i * 3).AddHours(18);
            db.Evento.Add(new Evento
            {
                Nome = titles[i] + " — Demonstração",
                Descricao = "Evento fictício criado automaticamente para apresentação do UniEvent. Não representa uma programação real.",
                Categoria = categories[i], DataEvento = date, Capacidade = 50 + i * 10,
                Thumbnail = new List<string> { ImagePath }, ResponsavelEventoId = owner.Id,
                InstituicaoId = owner.InstituicaoId, Local = "Auditório de demonstração",
                Visibilidade = VisibilidadeEvento.Publico, PublicoPermitido = PublicoPermitido.PublicoGeral,
                InicioInscricoes = now, FimInscricoes = date.AddHours(-1)
            });
        }
        await db.SaveChangesAsync(cancellationToken);
        await db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"__DeploymentSeeds\" (\"Id\", \"AppliedAtUtc\") VALUES ({SeedId}, {DateTime.UtcNow})",
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation("Carga {SeedId} aplicada: 6 eventos e 3 responsáveis.", SeedId);
    }
}