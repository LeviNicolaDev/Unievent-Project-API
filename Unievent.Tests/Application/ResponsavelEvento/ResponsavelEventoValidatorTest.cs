using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.ResponsavelEvento;
using Unievent.Application.Validators.ResponsavelEvento;
using Xunit;

namespace Unievent.Tests.Application.ResponsavelEvento;

public class ResponsavelEventoValidatorTest
{
    private readonly ResponsavelEventoRequestValidator _requestValidator;
    private readonly ResponsavelEventoUpdateValidator _updateValidator;
    public ResponsavelEventoValidatorTest()
    {
        _requestValidator = new ResponsavelEventoRequestValidator();
        _updateValidator = new ResponsavelEventoUpdateValidator();
    }

    [Fact]
    public async Task Deve_Validar_Dados_Com_Sucesso()
    {
        // Arrange
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

        var request = new ResponsavelEventoRequest
        {
            Nome = "João Silva",
            FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            }
        };
        // Act
        var result = await _requestValidator.ValidateAsync(request);
        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Dados_Invalidos_No_Update()
    {
        // Arrange
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

        var request = new ResponsavelEventoUpdate
        {
            Nome = "",
            FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            }
        };
        // Act
        var result = await _updateValidator.ValidateAsync(request);
        // Assert
        result.IsValid.Should().BeFalse();
    }


    [Fact]
    public async Task Deve_Validar_Dados_Com_Sucesso_No_Update()
    {
        // Arrange
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

        var request = new ResponsavelEventoUpdate
        {
            Nome = "João Silva",
            FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            }
        };
        // Act
        var result = await _updateValidator.ValidateAsync(request);
        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Dados_Invalidos()
    {
        // Arrange
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

        var request = new ResponsavelEventoRequest
        {
            Nome = "",
            FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            }
        };
        // Act
        var result = await _requestValidator.ValidateAsync(request);
        // Assert
        result.IsValid.Should().BeFalse();
    }
}
