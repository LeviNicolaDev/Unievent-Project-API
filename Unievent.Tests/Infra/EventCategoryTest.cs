using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Validators.Evento;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;
using Xunit;

namespace Unievent.Tests.Infra;

public class EventCategoryTest
{
    [Fact]
    public void ExistingCategoryCodes_ArePreserved()
    {
        Assert.Equal(0, (int)Categoria.Palestra);
        Assert.Equal(1, (int)Categoria.Workshop);
        Assert.Equal(2, (int)Categoria.Feira);
        Assert.Equal(3, (int)Categoria.Evento);
        Assert.Equal(4, (int)Categoria.Festa);
    }

    [Theory]
    [InlineData(Categoria.Curso, "curso")]
    [InlineData(Categoria.Musica, "musica")]
    [InlineData(Categoria.Hackathon, "hackathon")]
    public void NewCategories_AreAcceptedForCreateUpdateJsonAndPersistence(Categoria category, string jsonValue)
    {
        var create = new EventoRequestValidator().Validate(new EventoRequest
        {
            Nome = "Evento", Descricao = "Descricao", DataEvento = DateTime.UtcNow.AddDays(1),
            Capacidade = 10, ResponsavelEventoId = 1, Thumbnail = [], Categoria = category
        });
        Assert.DoesNotContain(create.Errors, error => error.PropertyName == "Categoria");
        var update = new EventoUpdateValidator().Validate(new EventoUpdate { Categoria = category });
        Assert.DoesNotContain(update.Errors, error => error.PropertyName == "Categoria");

        var json = new JsonSerializerOptions();
        json.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        Assert.Equal(category, JsonSerializer.Deserialize<Categoria>(JsonSerializer.Serialize(jsonValue), json));
        Assert.Equal(JsonSerializer.Serialize(jsonValue), JsonSerializer.Serialize(category, json));

        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=category_mapping;Username=test;Password=test").Options);
        var converter = db.Model.FindEntityType(typeof(Evento))!
            .FindProperty(nameof(Evento.Categoria))!.GetTypeMapping().Converter!;
        Assert.Equal(category.ToString(), converter.ConvertToProvider(category));
        Assert.Equal(category, converter.ConvertFromProvider(category.ToString()));
    }

    [Fact]
    public void UnknownCategories_AreStillRejected()
    {
        var result = new EventoUpdateValidator().Validate(new EventoUpdate { Categoria = (Categoria)999 });
        Assert.Contains(result.Errors, error => error.PropertyName == "Categoria");
    }
}
