using FluentAssertions;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Validators.UsuarioSecretaria;
using Unievent.Domain.Enuns;
using Xunit;

namespace Unievent.Tests.Application;

public class UsuarioSecretariaValidatorTest
{
    private readonly UsuarioSecretariaRequestValidator _validator;
    private readonly UsuarioSecretariaUpdateValidator _updateValidator;
    public UsuarioSecretariaValidatorTest()
    {
        _validator = new UsuarioSecretariaRequestValidator();
        _updateValidator = new UsuarioSecretariaUpdateValidator();
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Senha_For_Menor_Que_6_No_Request()
    {
        var request = new UsuarioSecretariaRequest
        {
            NomeUsuario = "Ryan",
            RoleUsuario = Role.Admin,
            EmailUsuario = "ryan@email.com",
            Senha = "123"
        };

        var result = await _validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("Ryan", Role.Admin, "ryan@fatec.sp.gov.br", "senha2")]
    public async Task Deve_Passar_Quando_Dados_Sao_Validos_No_Request(string nome, Role role, string email, string senha)
    {
        var request = new UsuarioSecretariaRequest
        {
            EmailUsuario = email,
            NomeUsuario = nome,
            RoleUsuario = role,
            Senha = senha
        };

        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }


    [Fact]

    public async Task Deve_Falhar_Quando_Email_Invalido_No_Request()
    {
        var request = new UsuarioSecretariaRequest
        {
            EmailUsuario = "teste@gmail.com",
            NomeUsuario = "ryan",
            RoleUsuario = Role.Admin,
            Senha = "senhaforte"
        };

        var result = await _validator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EmailUsuario" && e.ErrorMessage == "O email deve ser institucional");
    }


    [Theory]
    [InlineData("Ryan", Role.Admin, "ryan@fatec.sp.gov.br", "senha2")]
    public async Task Deve_Passar_Quando_Dados_Sao_Validos_No_Update(string nome, Role role, string email, string senha)
    {
        var request = new UsuarioSecretariaUpdate
        {
            EmailUsuario = email,
            NomeUsuario = nome,
            Role = role,
            Senha = senha
        };

        var result = await _updateValidator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]

    public async Task Deve_Falhar_Quando_Email_Invalido_No_Update()
    {
        var request = new UsuarioSecretariaUpdate
        {
            EmailUsuario = "teste@gmail.com",
            NomeUsuario = "ryan",
            Role = Role.Admin,
            Senha = "senhaforte"
        };

        var result = await _updateValidator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EmailUsuario" && e.ErrorMessage == "O email deve ser institucional");
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Senha_For_Menor_Que_6_No_Update()
    {
        var request = new UsuarioSecretariaUpdate
        {
            NomeUsuario = "Ryan",
            Role = Role.Admin,
            EmailUsuario = "ryan@email.com",
            Senha = "123"
        };

        var result = await _updateValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }



}

