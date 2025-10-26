using Calculator.Server.Models;
using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Calculator.Server.Services;

/// <summary>
/// IBM MQ consumer service for processing calculation requests (Simulated)
/// </summary>
public class IBMMQConsumerService : IIBMMQConsumerService
{
    private readonly IIBMMQConnectionService _connectionService;
    private readonly ICalculatorService _calculatorService;
    private readonly IBMMQConfiguration _config;
    private readonly ILogger<IBMMQConsumerService> _logger;
    private bool _disposed = false;
    private bool _isConsuming = false;

    public IBMMQConsumerService(
        IIBMMQConnectionService connectionService,
        ICalculatorService calculatorService,
        IOptions<IBMMQConfiguration> config,
        ILogger<IBMMQConsumerService> logger)
    {
        _connectionService = connectionService;
        _calculatorService = calculatorService;
        _config = config.Value;
        _logger = logger;
    }

    public void SetupQueues()
    {
        try
        {
            _logger.LogInformation("IBM MQ queues setup completed (simulated)");
            _logger.LogInformation("Request Queue: {RequestQueue}", _config.RequestQueueName);
            _logger.LogInformation("Response Queue: {ResponseQueue}", _config.ResponseQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup IBM MQ queues");
            throw;
        }
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!_connectionService.IsConnected)
            {
                await _connectionService.ConnectAsync();
            }

            _isConsuming = true;
            _logger.LogInformation("Started consuming messages from queue: {QueueName}", _config.RequestQueueName);

            while (!cancellationToken.IsCancellationRequested && _isConsuming)
            {
                try
                {
                    // Poll for messages
                    if (_connectionService.TryReceiveMessage(_config.RequestQueueName, out string? messageText))
                    {
                        await ProcessMessage(messageText);
                    }
                    else
                    {
                        // No message available, wait a bit
                        await Task.Delay(100, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while consuming messages");
                    await Task.Delay(1000, cancellationToken); // Wait before retrying
                }
            }

            _logger.LogInformation("Message consumption stopped");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Message consumption was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages");
            throw;
        }
    }

    public async Task StopConsumingAsync()
    {
        try
        {
            _isConsuming = false;
            _logger.LogInformation("Stopped consuming messages");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping message consumption");
        }
    }

    private async Task ProcessMessage(string messageText)
    {
        var correlationId = string.Empty;
        
        try
        {
            _logger.LogDebug("Received message: {Message}", messageText);

            var request = JsonSerializer.Deserialize<CalculationRequest>(messageText);
            if (request == null)
            {
                _logger.LogError("Failed to deserialize calculation request");
                return;
            }

            correlationId = request.CorrelationId;
            _logger.LogInformation("Processing calculation request with CorrelationId: {CorrelationId}", correlationId);

            // Process the calculation
            var response = await _calculatorService.CalculateAsync(request);

            // Send response back to client
            await SendResponseAsync(request.ReplyTo, response);
            
            _logger.LogInformation("Successfully processed calculation request with CorrelationId: {CorrelationId}", correlationId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message, CorrelationId: {CorrelationId}", correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message, CorrelationId: {CorrelationId}", correlationId);
            
            // Send error response
            try
            {
                var errorResponse = new CalculationResponse
                {
                    CorrelationId = correlationId,
                    Success = false,
                    ErrorMessage = "Internal server error occurred while processing the request"
                };

                var request = JsonSerializer.Deserialize<CalculationRequest>(messageText);
                if (request != null && !string.IsNullOrEmpty(request.ReplyTo))
                {
                    await SendResponseAsync(request.ReplyTo, errorResponse);
                }
            }
            catch (Exception responseEx)
            {
                _logger.LogError(responseEx, "Failed to send error response, CorrelationId: {CorrelationId}", correlationId);
            }
        }
    }

    private async Task SendResponseAsync(string replyToQueue, CalculationResponse response)
    {
        try
        {
            if (string.IsNullOrEmpty(replyToQueue))
            {
                _logger.LogWarning("No reply-to queue specified for response, CorrelationId: {CorrelationId}", response.CorrelationId);
                return;
            }

            var responseJson = JsonSerializer.Serialize(response);
            _connectionService.SendMessage(replyToQueue, responseJson);

            _logger.LogDebug("Sent response to queue {ReplyToQueue}, CorrelationId: {CorrelationId}", 
                replyToQueue, response.CorrelationId);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send response, CorrelationId: {CorrelationId}", response.CorrelationId);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            StopConsumingAsync().Wait(TimeSpan.FromSeconds(5));
            _logger.LogInformation("IBM MQ consumer service disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing IBM MQ consumer service");
        }
        finally
        {
            _disposed = true;
        }
    }
}