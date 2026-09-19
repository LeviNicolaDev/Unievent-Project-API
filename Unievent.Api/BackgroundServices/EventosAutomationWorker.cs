using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.BackgroundServices;

public class EventosAutomationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventosAutomationWorker> _logger;

    public EventosAutomationWorker(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<EventosAutomationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervaloMinutos = Math.Max(5, _configuration.GetValue("Automacoes:IntervaloMinutos", 30));
        var atrasoInicialSegundos = Math.Max(0, _configuration.GetValue("Automacoes:AtrasoInicialSegundos", 60));

        if (atrasoInicialSegundos > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(atrasoInicialSegundos), stoppingToken);
        }

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(intervaloMinutos));
        do
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IAutomacaoEventosService>();
                var result = await service.ProcessarAsync(stoppingToken);
                if (result.IsFailure)
                    _logger.LogWarning("Automação de eventos falhou: {Erros}", string.Join("; ", result.Errors));
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no worker de automações de eventos");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
