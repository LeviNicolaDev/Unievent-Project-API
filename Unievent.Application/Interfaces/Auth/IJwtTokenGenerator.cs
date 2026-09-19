using Unievent.Domain.Enuns;

namespace Unievent.Application.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    string GerarToken(int id, string email, Role role);
    string GerarToken(int id, string email, Role role, int? instituicaoId);
    string GerarToken(int id, string email, Role role, int? instituicaoId, StatusUsuarioSecretaria? status);
    string GerarToken(int id, string email, Role role, TipoParticipante tipoParticipante);
    string GerarToken(int id, string email, Role role, TipoParticipante tipoParticipante, int? instituicaoId);
}
