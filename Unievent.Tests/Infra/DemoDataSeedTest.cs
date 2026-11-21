using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Unievent.Api.Deployment;
using Unievent.Domain.Enuns;
using Unievent.Tests.Infrastructure;
using Xunit;

namespace Unievent.Tests.Infra;

public class DemoDataSeedTest
{
    [PostgreSqlFact]
    public async Task Seed_AppliesOnce_AndDoesNotRecreateDeletedEvents()
    {
        await using var database = new CertificacaoDatabase();
        await using var db = database.CreateDbContext();
        await db.Database.MigrateAsync();
        await DemoDataSeed.RunAsync(db, NullLogger.Instance);
        var events = await db.Evento.ToListAsync();
        events.Should().HaveCount(6);
        events.Should().OnlyContain(e => e.Visibilidade == VisibilidadeEvento.Publico
            && e.PublicoPermitido == PublicoPermitido.PublicoGeral
            && e.DataEvento > DateTime.UtcNow && e.InicioInscricoes <= DateTime.UtcNow
            && e.Thumbnail.Single() == DemoDataSeed.ImagePath && e.InstituicaoId != null);
        (await db.ResponsavelEvento.CountAsync()).Should().Be(3);
        db.Evento.Remove(events[0]);
        await db.SaveChangesAsync();
        await DemoDataSeed.RunAsync(db, NullLogger.Instance);
        (await db.Evento.CountAsync()).Should().Be(5);
        (await db.ResponsavelEvento.CountAsync()).Should().Be(3);
    }

    [PostgreSqlFact]
    public async Task Seed_ConcurrentReplicas_DoNotDuplicateData()
    {
        await using var database = new CertificacaoDatabase();
        await using var first = database.CreateDbContext();
        await using var second = database.CreateDbContext();
        await first.Database.MigrateAsync();
        await Task.WhenAll(DemoDataSeed.RunAsync(first, NullLogger.Instance),
            DemoDataSeed.RunAsync(second, NullLogger.Instance));
        (await first.Evento.CountAsync()).Should().Be(6);
        (await first.ResponsavelEvento.CountAsync()).Should().Be(3);
    }
}