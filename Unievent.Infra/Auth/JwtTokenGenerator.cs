using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Unievent.Application.Interfaces.Auth;
using Unievent.Domain.Enuns;

namespace Unievent.Infra.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(int id, string email, Role role)
        => GerarTokenInterno(id, email, role, null, null);

    public string GerarToken(int id, string email, Role role, int? instituicaoId)
        => GerarTokenInterno(id, email, role, null, instituicaoId);

    public string GerarToken(int id, string email, Role role, int? instituicaoId, StatusUsuarioSecretaria? status)
        => GerarTokenInterno(id, email, role, null, instituicaoId, status);

    public string GerarToken(int id, string email, Role role, TipoParticipante tipoParticipante)
        => GerarTokenInterno(id, email, role, tipoParticipante, null);

    public string GerarToken(int id, string email, Role role, TipoParticipante tipoParticipante, int? instituicaoId)
        => GerarTokenInterno(id, email, role, tipoParticipante, instituicaoId);

    private string GerarTokenInterno(int id, string email, Role role, TipoParticipante? tipoParticipante, int? instituicaoId, StatusUsuarioSecretaria? status = null)
    {
        var claims = new List<Claim>
        {
            new Claim (ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim("tipo_usuario", ObterTipoUsuario(role, tipoParticipante, instituicaoId))
        };
        if (tipoParticipante.HasValue)
            claims.Add(new Claim("tipo_participante", tipoParticipante.Value.ToString()));
        if (instituicaoId.HasValue)
            claims.Add(new Claim("instituicao_id", instituicaoId.Value.ToString()));
        if (status.HasValue)
            claims.Add(new Claim("status_usuario", status.Value.ToString()));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var tempoExpiracao = _configuration.GetValue<int>("Jwt:ExpirationMinutes");
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tempoExpiracao),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string ObterTipoUsuario(Role role, TipoParticipante? tipoParticipante, int? instituicaoId)
    {
        if (tipoParticipante.HasValue) return "Aluno";
        if (role == Role.Admin && !instituicaoId.HasValue) return "UsuarioUnievent";

        return "UsuarioSecretaria";
    }
}
