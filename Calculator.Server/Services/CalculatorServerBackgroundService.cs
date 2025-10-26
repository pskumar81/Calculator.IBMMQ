using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Calculator.Server.Services;

/// <summary>
/// Background service that manages the lifecycle of the IBM MQ consumer
/// </summary>
public class CalculatorServerBackgroundService : BackgroundService
{
    private readonly IIBMMQConsumerService _consumerService;
    private readonly ILogger<CalculatorServerBackgroundService> _logger;

    public CalculatorServerBackgroundService(
        IIBMMQConsumerService consumerService,
        ILogger<CalculatorServerBackgroundService> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Calculator Server Background Service is starting");

            // Setup IBM MQ queues
            _consumerService.SetupQueues();

            // Start consuming messages
            await _consumerService.StartConsumingAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Calculator Server Background Service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Calculator Server Background Service encountered an error");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculator Server Background Service is stopping");
        
        try
        {
            await _consumerService.StopConsumingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping consumer service");
        }

        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Calculator Server Background Service stopped");
    }
}