using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Api.Controllers;
using Unievent.Application.Common;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Enuns;
using Unievent.Infra.Services;
using Unievent.Tests.Infrastructure;
using Xunit;

namespace Unievent.Tests.Infra;

public class InterestRecommendationsTest
{
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    public async Task SavedInterests_ControlRecommendations_AndDoNotSendDuplicates(
        bool reminders, bool recommendations, bool expectedSend)
    {
        await using var database = new CertificacaoDatabase();
        await database.SeedAsync(certificado: false, presente: false);
        await using var db = database.CreateDbContext();
        var evento = await db.Evento.SingleAsync();
        evento.Categoria = Categoria.Palestra;
        evento.DataEvento = DateTime.UtcNow.AddDays(9);
        await db.SaveChangesAsync();
        var controller = new PreferenciasController(db)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        [new Claim(ClaimTypes.NameIdentifier, "1"), new Claim(ClaimTypes.Role, "Aluno")], "Test"))
                }
            }
        };
        var email = new Mock<IEmailService>();
        email.Setup(service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<bool>.Success(true));
        var automation = new AutomacaoEventosService(db, email.Object,
            Mock.Of<ICertificadoAutomaticoService>(), Mock.Of<ILogger<AutomacaoEventosService>>());

        Assert.IsType<OkObjectResult>(await controller.Atualizar(new PreferenciaRequest(
            reminders, true, recommendations, false, ["Workshop"], null, null)));
        Assert.True((await automation.ProcessarAsync()).IsSuccess);
        email.Verify(service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        Assert.IsType<OkObjectResult>(await controller.Atualizar(new PreferenciaRequest(
            reminders, true, recommendations, false, ["Palestra"], null, null)));
        Assert.Equal("Palestra", (await db.PreferenciaNotificacao.AsNoTracking().SingleAsync()).Categorias);
        var sent = await automation.ProcessarAsync();
        Assert.True(sent.IsSuccess);
        Assert.Equal(expectedSend ? 1 : 0, sent.Value.AlertasProcessados);
        var repeated = await automation.ProcessarAsync();
        Assert.True(repeated.IsSuccess);
        Assert.Equal(0, repeated.Value.AlertasProcessados);
        email.Verify(service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            expectedSend ? Times.Once() : Times.Never());
    }
}
