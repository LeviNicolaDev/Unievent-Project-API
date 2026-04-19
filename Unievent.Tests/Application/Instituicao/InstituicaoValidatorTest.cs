using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.Instituicao;
using Unievent.Application.Validators.Instituicao;
using Xunit;

namespace Unievent.Tests.Application.Instituicao;

public class InstituicaoValidatorTest
{
    private readonly InstituicaoRequestValidator _requestValidator;
    private readonly InstituicaoUpdateValidator _updateValidator;
    public InstituicaoValidatorTest()
    {
        _requestValidator = new InstituicaoRequestValidator();
        _updateValidator = new InstituicaoUpdateValidator();
    }

    [Fact]
    public async Task Deve_Validar_Dados_Com_Sucesso_No_Request()
    {
        // Arrange
         using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
        var request = new InstituicaoRequest
        {
            Cnpj= "16203686000158",
            EmailLogin="teste@fatec.sp.gov.br",
            EnderecoId=1,
            SenhaLogin="teste123",
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
        var request = new InstituicaoUpdate
        {
            Cnpj= "162036860001587643677463678324s",
            EmailLogin="teste@fatec.sp.gov.br",
            EnderecoId=1,
            SenhaLogin="teste123",
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
        var request = new InstituicaoUpdate
        {
            Cnpj= "16203686000158",
            EmailLogin="teste@fatec.sp.gov.br",
            EnderecoId=1,
            SenhaLogin="teste123",
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
    public async Task Deve_Falhar_Quando_Dados_Invalidos_No_Request()
    {
        // Arrange
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
        var request = new InstituicaoRequest
        {
            Cnpj= "162036860001587643784536853487634",
            EmailLogin="teste@gmail.com",
            EnderecoId=1,
            SenhaLogin="teste123",
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
