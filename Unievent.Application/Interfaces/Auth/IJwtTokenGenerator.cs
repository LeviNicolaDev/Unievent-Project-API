using Unievent.Domain.Enuns;

namespace Unievent.Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    string GerarToken(int id, string email, Role role);
    string GerarToken(int id, string email, Role role, TipoParticipante tipoParticipante);
}
