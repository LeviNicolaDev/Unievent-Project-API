using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Validators.Evento;
using Xunit;

namespace Unievent.Tests.Application.Evento;

public class EventoValidatorTest
{
    private readonly EventoRequestValidator _requestValidator;
    private readonly EventoUpdateValidator _updateValidator;
    public EventoValidatorTest()
    {
        _requestValidator = new EventoRequestValidator();
        _updateValidator = new EventoUpdateValidator();

    }
    [Fact]
    public async Task Deve_Validar_Dados_Com_Sucesso()
    {
        var request = new EventoRequest
        {
            Nome = "Evento de Teste",
            Descricao = "Descrição do evento de teste",
            DataEvento = DateTime.Now.AddDays(10),
            Categoria = Domain.Enuns.Categoria.Feira,
            ResponsavelEventoId = 1,
            Capacidade = 100,
            Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
        };

        var result = await _requestValidator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Deve_Validar_Dados_De_Atualizacao_Com_Sucesso()
    {
        var update = new EventoUpdate
        {
            Nome = "Evento de Teste",
            Descricao = "Descrição do evento de teste",
            DataEvento = DateTime.Now.AddDays(10),
            Categoria = Domain.Enuns.Categoria.Palestra,
            Capacidade = 100,
            Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },

        };

        var result = await _updateValidator.ValidateAsync(update);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Dados_Foram_Informados_Incorretamente()
    {
        var request = new EventoRequest
        {
            Nome = "",
            Descricao = "",
            DataEvento = DateTime.Now.AddDays(-1),
            Categoria = Domain.Enuns.Categoria.Palestra,
            ResponsavelEventoId = 65,
            Capacidade = 72,
            Thumbnail = null,
        };
        var result = await _requestValidator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
    }
    [Fact]
    public async Task Deve_Lancar_Erro_Quando_Dados_De_Atualizacao_Foram_Informados_Incorretamente()
    {
        var update = new EventoUpdate
        {
            Nome = "Evento de Teste",
            Descricao = "Descrição do evento de teste",
            DataEvento = DateTime.Now.AddDays(10),
            Categoria = Domain.Enuns.Categoria.Palestra,
            Capacidade = -1,
            Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },

        };

        var result = await _updateValidator.ValidateAsync(update);
        result.IsValid.Should().BeFalse();
    }
}
