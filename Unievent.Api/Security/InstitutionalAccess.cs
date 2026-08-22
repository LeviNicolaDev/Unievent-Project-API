using System.Security.Claims;
using Unievent.Domain.Enuns;

namespace Unievent.Api.Security;

public static class InstitutionalAccess
{
    public const string InstituicaoClaim = "instituicao_id";

    public static int? GetInstituicaoId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(InstituicaoClaim)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

    public static bool IsGlobalAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole(Role.Admin.ToString()) && user.GetInstituicaoId() is null;
    }

    public static bool CanAccessInstituicao(this ClaimsPrincipal user, int? instituicaoId)
    {
        if (user.IsGlobalAdmin()) return true;

        var usuarioInstituicaoId = user.GetInstituicaoId();
        return usuarioInstituicaoId.HasValue &&
               instituicaoId.HasValue &&
               usuarioInstituicaoId.Value == instituicaoId.Value;
    }
}
