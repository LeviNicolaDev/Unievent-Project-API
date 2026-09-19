using Unievent.Domain.Enuns;

namespace Unievent.Application.Dtos.Participacao;

public record IngressoResponse
{
    public int Id { get; init; }
    public int EventoId { get; init; }
    public string NomeEvento { get; init; } = string.Empty;
    public string InstituicaoNome { get; init; } = string.Empty;
    public DateTime DataEvento { get; init; }
    public string Local { get; init; } = string.Empty;
    public StatusInscricao StatusInscricao { get; init; }
    public bool PresencaConfirmada { get; init; }
    public DateTime? DataCheckIn { get; init; }
    public string CodigoIngresso { get; init; } = string.Empty;
    public string ConteudoQrCode => CodigoIngresso;
    public bool EventoRealizado { get; init; }
    public bool PodeRealizarCheckIn => StatusInscricao == StatusInscricao.Ativa && !PresencaConfirmada;
}
