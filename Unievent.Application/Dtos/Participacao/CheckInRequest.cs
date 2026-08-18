namespace Unievent.Application.Dtos.Participacao;

public record CheckInRequest
{
    public required string CodigoIngresso { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double PrecisaoMetros { get; init; }
}
