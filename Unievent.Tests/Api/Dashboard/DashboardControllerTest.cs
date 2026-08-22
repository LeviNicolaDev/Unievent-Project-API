using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unievent.Api.Controllers;
using Unievent.Api.Security;
using Unievent.Domain.Enuns;
using Xunit;

namespace Unievent.Tests.Api.Dashboard;

public class DashboardControllerTest
{
    [Fact]
    public async Task AdminDashboard_Deve_Bloquear_Admin_Com_Instituicao()
    {
        var controller = CriarController(Role.Admin, instituicaoId: 1);

        var result = await controller.AdminDashboard();

        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task InstituicaoDashboard_Deve_Bloquear_Secretaria_Sem_Instituicao()
    {
        var controller = CriarController(Role.Secretaria);

        var result = await controller.InstituicaoDashboard();

        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public void Secretaria_Nao_Deve_Acessar_Outra_Instituicao()
    {
        var user = CriarUsuario(Role.Secretaria, instituicaoId: 1);

        user.CanAccessInstituicao(1).Should().BeTrue();
        user.CanAccessInstituicao(2).Should().BeFalse();
    }

    private static DashboardController CriarController(Role role, int? instituicaoId = null)
    {
        return new DashboardController(null!)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CriarUsuario(role, instituicaoId)
                }
            }
        };
    }

    private static ClaimsPrincipal CriarUsuario(Role role, int? instituicaoId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role.ToString())
        };
        if (instituicaoId.HasValue)
        {
            claims.Add(new Claim(InstitutionalAccess.InstituicaoClaim, instituicaoId.Value.ToString()));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}
