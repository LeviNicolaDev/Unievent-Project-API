using Microsoft.EntityFrameworkCore;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;
using Xunit;

namespace Unievent.Tests.Infra;

public class DateTimeMappingTest
{
    [Theory]
    [InlineData(DateTimeKind.Unspecified)]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Utc)]
    public void AllTimestamps_UseUtcAndPreserveTime(DateTimeKind kind)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=mapping_test;Username=test;Password=test")
            .Options;
        using var context = new AppDbContext(options);
        var input = new DateTime(2026, 11, 29, 12, 23, 45, kind).AddTicks(1230);
        var expected = kind == DateTimeKind.Local
            ? input.ToUniversalTime()
            : DateTime.SpecifyKind(input, DateTimeKind.Utc);
        var properties = context.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetProperties())
            .Where(property => property.ClrType == typeof(DateTime)
                || property.ClrType == typeof(DateTime?))
            .Where(property => !(property.DeclaringType.ClrType == typeof(Aluno)
                && property.Name == nameof(Aluno.DataNascimento)))
            .ToArray();

        Assert.NotEmpty(properties);
        foreach (var property in properties)
        {
            var propertyExpected = property.DeclaringType.ClrType == typeof(Evento)
                && kind == DateTimeKind.Unspecified
                ? TimeZoneInfo.ConvertTimeToUtc(input,
                    TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"))
                : expected;
            Assert.Equal("timestamp with time zone", property.GetColumnType());
            var converter = property.GetTypeMapping().Converter;
            Assert.NotNull(converter);
            var stored = Assert.IsType<DateTime>(converter.ConvertToProvider(input));
            Assert.Equal(DateTimeKind.Utc, stored.Kind);
            Assert.Equal(propertyExpected.Ticks, stored.Ticks);
            var loaded = Assert.IsType<DateTime>(converter.ConvertFromProvider(stored));
            Assert.Equal(DateTimeKind.Utc, loaded.Kind);
            Assert.Equal(propertyExpected.Ticks, loaded.Ticks);
            if (property.ClrType == typeof(DateTime?))
            {
                Assert.Null(converter.ConvertToProvider(null));
                Assert.Null(converter.ConvertFromProvider(null));
            }
        }
    }
}
