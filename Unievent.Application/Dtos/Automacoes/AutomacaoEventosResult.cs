namespace Unievent.Application.Dtos.Automacoes;

public record AutomacaoEventosResult(
    int CertificadosProcessados,
    int AlertasProcessados,
    int Falhas);
