using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Unievent.Domain.Enuns;
using Unievent.Infra.Migrations;
using Unievent.Tests.Infrastructure;
using Xunit;

namespace Unievent.Tests.Infra;

public class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UNIEVENT_TEST_SQLSERVER")))
            Skip = "Requer UNIEVENT_TEST_SQLSERVER; usa banco temporário isolado.";
    }
}

public class CertificadoMigrationTest
{
    [SqlServerFact]
    public async Task Migration_Incremental_Preserva_Presenca_Codigo_Envio_Legado()
    {
        await using var database = new CertificacaoDatabase();
        await database.SeedAsync();
        await using var db = database.CreateDbContext();
        var p = await db.Participacao.SingleAsync();
        p.EmitirCertificado("codigo-original");
        p.RegistrarEnvioCertificadoEmail();
        await db.SaveChangesAsync();
        var migration = new AddCertificadoPdfDelivery();
        var generator = db.GetService<IMigrationsSqlGenerator>();
        // Neste banco de teste, retira somente as colunas novas para reproduzir o schema anterior.
        foreach (var command in generator.Generate(migration.DownOperations, db.Model))
            await db.Database.ExecuteSqlRawAsync(command.CommandText);
        foreach (var command in generator.Generate(migration.UpOperations, db.Model))
            await db.Database.ExecuteSqlRawAsync(command.CommandText);
        db.ChangeTracker.Clear();
        var saved = await db.Participacao.SingleAsync();
        saved.PresencaConfirmada.Should().BeTrue();
        saved.CodigoValidacao.Should().Be("codigo-original");
        saved.CertificadoEnviadoPorEmail.Should().BeTrue();
        saved.StatusEnvioCertificado.Should().Be(StatusEnvioCertificado.Enviado);
        saved.CertificadoPdf.Should().BeNull();
    }
}
