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
        => GerarTokenInterno(id, email, role, null);

    public string GerarToken(int id, string email, Role role, TipoParticipante tipoParticipante)
        => GerarTokenInterno(id, email, role, tipoParticipante);

    private string GerarTokenInterno(int id, string email, Role role, TipoParticipante? tipoParticipante)
    {
        var claims = new List<Claim>
        {
            new Claim (ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role.ToString())
        };
        if (tipoParticipante.HasValue)
            claims.Add(new Claim("tipo_participante", tipoParticipante.Value.ToString()));

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
}
