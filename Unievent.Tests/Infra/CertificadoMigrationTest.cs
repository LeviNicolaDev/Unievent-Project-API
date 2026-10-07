using FluentAssertions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Unievent.Tests.Infrastructure;
using Xunit;

namespace Unievent.Tests.Infra;

public class PostgreSqlFactAttribute : FactAttribute
{
    public PostgreSqlFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UNIEVENT_TEST_POSTGRESQL")))
            Skip = "Requer UNIEVENT_TEST_POSTGRESQL; usa banco temporário isolado.";
    }
}

public class CertificadoMigrationTest
{
    [PostgreSqlFact]
    public async Task Migration_PostgreSql_Cria_Schema()
    {
        await using var database = new CertificacaoDatabase();
        await using var db = database.CreateDbContext();

        await db.Database.MigrateAsync();

        (await db.Database.GetAppliedMigrationsAsync())
            .Should()
            .ContainSingle(migration => migration.EndsWith("_InitialPostgreSQL", StringComparison.Ordinal));
    }
}
