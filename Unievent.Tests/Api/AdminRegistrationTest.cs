using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Unievent.Api.Controllers;
using Unievent.Application.Common;
using Unievent.Application.Dtos.UsuarioUnievent;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Xunit;

namespace Unievent.Tests.Api;

public class AdminRegistrationTest
{
    [Theory]
    [InlineData("admin@fatec.sp.gov.br", true)]
    [InlineData("ADMIN@ALUNO.CPS.SP.GOV.BR", true)]
    [InlineData("admin@aluno.sp.gov.br", false)]
    [InlineData("admin@fatec.sp.gov.br.evil.com", false)]
    [InlineData("a@b@fatec.sp.gov.br", false)]
    [InlineData("admin@example.com", false)]
    public async Task Cadastro_Valida_Dominio_Nos_Dois_Endpoints(string email, bool valid)
    {
        foreach (var authenticated in new[] { false, true })
        {
            var repo = new Mock<IUsuarioUnieventRepository>();
            var mail = new Mock<IEmailService>();
            var saved = false;
            repo.Setup(r => r.ExisteUsuarioUnievent()).ReturnsAsync(true);
            repo.Setup(r => r.SaveChangesAsync()).Callback(() => saved = true).Returns(Task.CompletedTask);
            mail.Setup(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback(() => Assert.True(saved)).ReturnsAsync(Result<bool>.Success(true));
            var controller = new UsuarioUnieventController(repo.Object, Mock.Of<IUsuarioSecretariaRepository>(), mail.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") }, "test"));
            var request = new UsuarioUnieventRequest { NomeUsuario = "Admin", EmailUsuario = email, Senha = "senha123" };
            var result = authenticated ? await controller.CriarAdminUnievent(request) : await controller.CriarAdminInicial(request);
            if (valid) Assert.IsType<OkObjectResult>(result);
            else Assert.IsType<BadRequestObjectResult>(result);
            repo.Verify(r => r.ExisteUsuarioUnievent(), Times.Never);
            mail.Verify(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), valid ? Times.Once() : Times.Never());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Cadastro_Nao_Envia_Email_Se_Gravacao_Falhar(bool duplicate)
    {
        var repo = new Mock<IUsuarioUnieventRepository>();
        var mail = new Mock<IEmailService>();
        if (duplicate)
            repo.Setup(r => r.ListarUsuarioUnieventByEmail(It.IsAny<string>())).ReturnsAsync(new UsuarioUnievent { NomeUsuario = "Existente", EmailUsuario = "admin@fatec.sp.gov.br", Senha = "hash" });
        else
            repo.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new Exception("database failed"));
        var controller = new UsuarioUnieventController(repo.Object, Mock.Of<IUsuarioSecretariaRepository>(), mail.Object);
        var request = new UsuarioUnieventRequest { NomeUsuario = "Admin", EmailUsuario = "admin@fatec.sp.gov.br", Senha = "senha123" };
        if (duplicate) Assert.IsType<BadRequestObjectResult>(await controller.CriarAdminInicial(request));
        else await Assert.ThrowsAsync<Exception>(() => controller.CriarAdminInicial(request));
        mail.Verify(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }
}
