using Calculator.Server.Models;

namespace Calculator.Server.Services.Interfaces;

/// <summary>
/// Interface for IBM MQ consumer service
/// </summary>
public interface IIBMMQConsumerService : IDisposable
{
    /// <summary>
    /// Sets up the IBM MQ queues for consumption
    /// </summary>
    void SetupQueues();

    /// <summary>
    /// Starts consuming messages from the request queue
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StartConsumingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops consuming messages
    /// </summary>
    Task StopConsumingAsync();
}