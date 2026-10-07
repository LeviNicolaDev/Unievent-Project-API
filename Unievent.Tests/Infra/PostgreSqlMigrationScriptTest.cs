using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Unievent.Infra.Data;
using Xunit;

namespace Unievent.Tests.Infra;

public class PostgreSqlMigrationScriptTest
{
    [Fact]
    public void InitialMigration_QuotesPartialIndexColumnsForPostgreSql()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=script_only;Username=unused;Password=unused")
            .Options;
        using var db = new AppDbContext(options);
        var script = db.GetService<IMigrator>().GenerateScript();

        script.Should().Contain("WHERE \"ChaveConfirmacaoEmail\" IS NOT NULL");
        script.Should().Contain("WHERE \"Codigo\" IS NOT NULL");
        script.Should().NotContain("[ChaveConfirmacaoEmail]");
        script.Should().NotContain("[Codigo]");
        db.Database.HasPendingModelChanges().Should().BeFalse();
    }
}
