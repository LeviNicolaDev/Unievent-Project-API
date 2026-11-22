using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Dtos.Auth;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Auth;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Unievent.Domain.Entities;
using Xunit;

namespace Unievent.Tests.Infra;

public class RegistrationEmailOrderTest
{
    private static Mock<IValidator<T>> Validator<T>() where T : class
    {
        var mock = new Mock<IValidator<T>>();
        mock.Setup(v => v.ValidateAsync(It.IsAny<T>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        return mock;
    }

    [Theory]
    [InlineData("aluno", "create")]
    [InlineData("aluno", "save")]
    [InlineData("aluno", "success")]
    [InlineData("externo", "create")]
    [InlineData("externo", "save")]
    [InlineData("externo", "success")]
    [InlineData("secretaria", "create")]
    [InlineData("secretaria", "save")]
    [InlineData("secretaria", "success")]
    public async Task Email_IsSentOnlyAfterSuccessfulSave(string profile, string outcome)
    {
        var saved = false;
        var alunos = new Mock<IAlunoRepository>();
        var secretarias = new Mock<IUsuarioSecretariaRepository>();
        var email = new Mock<IEmailService>();
        email.Setup(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback(() => Assert.True(saved, "Confirmation must follow persistence"))
            .ReturnsAsync(Result<bool>.Success(true));
        alunos.Setup(r => r.CriarAluno(It.IsAny<Aluno>())).Returns<Aluno>(a =>
            outcome == "create" ? Task.FromException<Aluno>(new Exception("create failed")) : Task.FromResult(a));
        secretarias.Setup(r => r.CriarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>())).Returns<UsuarioSecretaria>(a =>
            outcome == "create" ? Task.FromException<UsuarioSecretaria>(new Exception("create failed")) : Task.FromResult(a));
        Task Save()
        {
            if (outcome == "save") return Task.FromException(new Exception("save failed"));
            saved = true;
            return Task.CompletedTask;
        }
        alunos.Setup(r => r.SaveChangesAsync()).Returns(Save);
        secretarias.Setup(r => r.SaveChangesAsync()).Returns(Save);

        bool success;
        if (profile == "aluno")
        {
            using var stream = new MemoryStream(new byte[] { 1 });
            var request = new AlunoRequest
            {
                Nome = "Aluno", Email = "aluno@fatec.sp.gov.br", Senha = "senha123",
                DataNascimento = new DateTime(2000, 1, 1),
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
            };
            IAlunoService service = new AlunoService(alunos.Object, Mock.Of<ILogger<AlunoService>>(),
                Validator<AlunoRequest>().Object, Validator<AlunoUpdate>().Object, email.Object);
            success = (await service.CriarAluno(request)).IsSuccess;
        }
        else if (profile == "externo")
        {
            var service = new AuthService(Mock.Of<IUsuarioUnieventRepository>(), secretarias.Object,
                Mock.Of<IJwtTokenGenerator>(), alunos.Object, Mock.Of<ILogger<AuthService>>(),
                Validator<PublicoGeralCadastroRequest>().Object, email.Object);
            success = (await service.CadastrarPublicoGeral(new PublicoGeralCadastroRequest
            {
                Nome = "Externo", Email = "externo@example.com", Senha = "senha123"
            })).IsSuccess;
        }
        else
        {
            IUsuarioSecretariaService service = new UsuarioSecretariaService(secretarias.Object,
                Mock.Of<ILogger<UsuarioSecretariaService>>(), Validator<UsuarioSecretariaRequest>().Object,
                Validator<UsuarioSecretariaUpdate>().Object, email.Object);
            success = (await service.CriarUsuarioSecretaria(new UsuarioSecretariaRequest
            {
                NomeUsuario = "Secretaria", EmailUsuario = "secretaria@fatec.sp.gov.br", Senha = "senha123"
            })).IsSuccess;
        }
        Assert.Equal(outcome == "success", success);
        email.Verify(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            outcome == "success" ? Times.Once() : Times.Never());
    }
}
