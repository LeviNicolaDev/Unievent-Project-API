using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Validators.Aluno;
using Unievent.Domain.Enuns;
using Xunit;

namespace Unievent.Tests.Application.Aluno;

public class AlunoValidatorTest
{
    private readonly AlunoRequestValidator _validatorRequest;
    private readonly AlunoUpdateValidator _validatorUpdate;
    public AlunoValidatorTest()
    {
        _validatorRequest = new AlunoRequestValidator();
        _validatorUpdate = new AlunoUpdateValidator();
    }

    [Theory]
    [InlineData("aluno@fatec.sp.gov.br", true)]
    [InlineData("aluno@aluno.cps.sp.gov.br", true)]
    [InlineData("ALUNO@ALUNO.CPS.SP.GOV.BR", true)]
    [InlineData("aluno@gmail.com", false)]
    [InlineData("aluno@fatec.sp.gov.br.evil.com", false)]
    [InlineData("aluno@sub.aluno.cps.sp.gov.br", false)]
    [InlineData("a@b@aluno.cps.sp.gov.br", false)]
    [InlineData("@aluno.cps.sp.gov.br", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public async Task Cadastro_Interno_Valida_Dominios_Institucionais(string? email, bool valido)
    {
        var request = new AlunoRequest
        {
            Nome = "Aluno", Email = email!,
            DataNascimento = new DateTime(2000, 1, 1), Senha = "senha123",
            TipoParticipante = TipoParticipante.Interno, InstituicaoId = 1,
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };
        var result = await _validatorRequest.ValidateAsync(request);
        Assert.Equal(valido, result.IsValid);
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Email_For_Invalido_No_Request()
    {
        // Arrange
        var request = new AlunoRequest
        {
            Nome = "Ryan",
            Email = "ryan@email.com",
            DataNascimento = new DateTime(2000, 1, 1),
            Senha = "senha123",
            TipoParticipante = TipoParticipante.Interno,
            InstituicaoId = 1,
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };
        // Act
        var result = await _validatorRequest.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Senha_For_Menor_Que_6_Caracteres_No_Request()
    {
        // Arrange
        var request = new AlunoRequest
        {
            Nome = "Ryan",
            Email = "ryan@fatec.sp.gov.br",
            DataNascimento = new DateTime(2000, 1, 1),
            Senha = "123",
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };
        // Act
        var result = await _validatorRequest.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_Passar_Quando_Dados_Forem_Validos_No_Request()
    {
        // Arrange
        var request = new AlunoRequest
        {
            Nome = "Ryan",
            Email = "ryan@fatec.sp.gov.br",
            DataNascimento = new DateTime(2000, 1, 1),
            Senha = "senha123",
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };
        // Act
        var result = await _validatorRequest.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Aluno_Interno_Nao_Informar_Instituicao()
    {
        var request = new AlunoRequest
        {
            Nome = "Ryan",
            Email = "ryan@fatec.sp.gov.br",
            DataNascimento = new DateTime(2000, 1, 1),
            Senha = "senha123",
            TipoParticipante = TipoParticipante.Interno,
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };

        var result = await _validatorRequest.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.ErrorMessage)
            .Should().Contain("A instituição é obrigatória para participantes internos");
    }

    [Fact]
    public async Task Deve_Passar_Quando_Aluno_Interno_Informar_Instituicao()
    {
        var request = new AlunoRequest
        {
            Nome = "Ryan",
            Email = "ryan@fatec.sp.gov.br",
            DataNascimento = new DateTime(2000, 1, 1),
            Senha = "senha123",
            TipoParticipante = TipoParticipante.Interno,
            InstituicaoId = 1,
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };

        var result = await _validatorRequest.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Deve_Falhar_Quando_Senha_For_Menor_Que_6_Caracteres_No_Update()
    {
        // Arrange
        var request = new AlunoUpdate
        {
            Nome = "Ryan",
            DataNascimento = new DateTime(2000, 1, 1),
            Senha = "sen",
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };
        // Act
        var result = await _validatorUpdate.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
    }


    [Fact]
    public async Task Deve_Passar_Quando_Dados_Forem_Validos_No_Update()
    {
        // Arrange
        var request = new AlunoUpdate
        {
            Nome = "Ryan",
            DataNascimento = new DateTime(2000, 1, 1),
            Senha = "senha123",
            FotoPerfil = new FormFile(null, 0, 0, null, "foto.jpg")
        };
        // Act
        var result = await _validatorUpdate.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

}
