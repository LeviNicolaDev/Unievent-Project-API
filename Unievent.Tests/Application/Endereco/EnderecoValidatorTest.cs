using FluentAssertions;
using Unievent.Application.Dtos.Endereco;
using Unievent.Application.Validators.Endereco;
using Xunit;

namespace Unievent.Tests.Application.Endereco;

public class EnderecoValidatorTest
{
    private readonly EnderecoRequestValidator _requestValidator;
    private readonly EnderecoUpdateValidator _updateValidator;
    public EnderecoValidatorTest()
    {
        _requestValidator = new EnderecoRequestValidator();
        _updateValidator = new EnderecoUpdateValidator();
    }

    [Fact]
    public async Task Deve_Validar_Dados_Com_Sucesso()
    {
        // Arrange
        var request = new EnderecoRequest
        {
            Rua = "Rua Exemplo",
            Numero = "123",
            Bairro = "Bairro Exemplo",
            Cidade = "Cidade Exemplo",
            Estado = "Estado Exemplo",
            Cep = "12345678"
        };
        // Act
        var result = await _requestValidator.ValidateAsync(request);
        // Assert
        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task Deve_Falhar_Quando_Dados_Invalidos()
    {
        // Arrange
        var request = new EnderecoRequest
        {
            Rua = "",
            Numero = "123674363482",
            Bairro = "Bairro Exemplo",
            Cidade = "Cidade Exemplo",
            Estado = "Estado Exemplo",
            Cep = "1234567823"
        };
        // Act
        var result = await _requestValidator.ValidateAsync(request);
        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_Validar_Dados_Com_Sucesso_No_Update()
    {
        // Arrange
        var update = new EnderecoUpdate
        {
            Rua = "rua exemplo",
            Numero = "123",
            Bairro = "Bairro Exemplo",
            Cidade = "Cidade Exemplo",
            Estado = "Estado Exemplo",
            Cep = "12345678"
        };
        // Act
        var result = await _updateValidator.ValidateAsync(update);
        // Assert
        result.IsValid.Should().BeTrue();
    }

     [Fact]
    public async Task Deve_Falhar_Quando_Dados_Invalidos_No_Update()
    {
        // Arrange
        var update = new EnderecoUpdate
        {
            Rua = "",
            Numero = "123674363482",
            Bairro = "Bairro Exemplo",
            Cidade = "Cidade Exemplo",
            Estado = "Estado Exemplo",
            Cep = "1234567823"
        };
        // Act
        var result = await _updateValidator.ValidateAsync(update);
        // Assert
        result.IsValid.Should().BeFalse();
    }
}
