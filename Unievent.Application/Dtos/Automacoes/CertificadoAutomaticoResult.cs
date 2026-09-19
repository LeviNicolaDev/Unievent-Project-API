namespace Unievent.Application.Dtos.Automacoes;

public record CertificadoAutomaticoResult(
    bool CertificadoConfigurado,
    bool CertificadoEmitido,
    bool EmailEnviado,
    bool EmailPendente,
    string? ErroEmail);
