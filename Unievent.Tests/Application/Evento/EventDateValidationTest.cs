using Unievent.Application.Dtos.Evento;
using Unievent.Application.Validators.Evento;
using Xunit;

namespace Unievent.Tests.Application.Evento;

public class EventDateValidationTest
{
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    public void CreateAndUpdate_AcceptTodayAndFuture_RejectPreviousDays(int days, bool accepted)
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone).Date;
        var civil = DateTime.SpecifyKind(today.AddDays(days), DateTimeKind.Unspecified);
        foreach (var value in new[] { civil, TimeZoneInfo.ConvertTimeToUtc(civil, zone) })
        {
            var create = new EventoRequestValidator().Validate(new EventoRequest
            {
                Nome = "Evento", Descricao = "Descricao", Capacidade = 10,
                ResponsavelEventoId = 1, Thumbnail = [], DataEvento = value
            });
            Assert.Equal(accepted, !create.Errors.Any(e => e.PropertyName == "DataEvento"));
            var update = new EventoUpdateValidator().Validate(new EventoUpdate { DataEvento = value });
            Assert.Equal(accepted, !update.Errors.Any(e => e.PropertyName == "DataEvento"));
        }
    }
}
