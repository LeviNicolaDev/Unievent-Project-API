using Microsoft.EntityFrameworkCore;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;
using Xunit;

namespace Unievent.Tests.Infra;

public class AlunoBirthDateMappingTest
{
    [Theory]
    [InlineData(DateTimeKind.Unspecified)]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Utc)]
    public void BirthDate_UsesUtcForPostgres_WithoutChangingCalendarDate(DateTimeKind kind)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=mapping_test;Username=test;Password=test")
            .Options;
        using var context = new AppDbContext(options);
        var property = context.Model.FindEntityType(typeof(Aluno))!
            .FindProperty(nameof(Aluno.DataNascimento))!;
        Assert.Equal("timestamp with time zone", property.GetColumnType());
        var converter = property.GetTypeMapping().Converter!;
        var input = new DateTime(2000, 2, 29, 23, 30, 0, kind);
        var stored = (DateTime)converter.ConvertToProvider(input)!;
        Assert.Equal(DateTimeKind.Utc, stored.Kind);
        Assert.Equal(new DateTime(2000, 2, 29), stored.Date);
        Assert.Equal(TimeSpan.Zero, stored.TimeOfDay);
        var loaded = (DateTime)converter.ConvertFromProvider(stored)!;
        Assert.Equal(input.Date, loaded);
        Assert.Equal(DateTimeKind.Unspecified, loaded.Kind);
    }
}
