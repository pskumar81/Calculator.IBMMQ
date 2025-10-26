using Calculator.Client.Models;
using Calculator.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Calculator.Client.Services;

/// <summary>
/// Service for sending calculation requests to the server via IBM MQ (Simulated)
/// </summary>
public class CalculatorClientService : ICalculatorClientService
{
    private readonly IIBMMQConnectionService _connectionService;
    private readonly IBMMQConfiguration _config;
    private readonly ILogger<CalculatorClientService> _logger;

    public CalculatorClientService(
        IIBMMQConnectionService connectionService,
        IOptions<IBMMQConfiguration> config,
        ILogger<CalculatorClientService> logger)
    {
        _connectionService = connectionService;
        _config = config.Value;
        _logger = logger;
    }

    public async Task<CalculationResponse> AddAsync(double operand1, double operand2)
    {
        var request = new CalculationRequest
        {
            Operand1 = operand1,
            Operand2 = operand2,
            Operation = CalculationOperation.Add,
            CorrelationId = Guid.NewGuid().ToString(),
            ReplyTo = _config.ResponseQueueName
        };

        return await SendCalculationRequestAsync(request);
    }

    public async Task<CalculationResponse> SubtractAsync(double operand1, double operand2)
    {
        var request = new CalculationRequest
        {
            Operand1 = operand1,
            Operand2 = operand2,
            Operation = CalculationOperation.Subtract,
            CorrelationId = Guid.NewGuid().ToString(),
            ReplyTo = _config.ResponseQueueName
        };

        return await SendCalculationRequestAsync(request);
    }

    public async Task<CalculationResponse> MultiplyAsync(double operand1, double operand2)
    {
        var request = new CalculationRequest
        {
            Operand1 = operand1,
            Operand2 = operand2,
            Operation = CalculationOperation.Multiply,
            CorrelationId = Guid.NewGuid().ToString(),
            ReplyTo = _config.ResponseQueueName
        };

        return await SendCalculationRequestAsync(request);
    }

    public async Task<CalculationResponse> DivideAsync(double operand1, double operand2)
    {
        var request = new CalculationRequest
        {
            Operand1 = operand1,
            Operand2 = operand2,
            Operation = CalculationOperation.Divide,
            CorrelationId = Guid.NewGuid().ToString(),
            ReplyTo = _config.ResponseQueueName
        };

        return await SendCalculationRequestAsync(request);
    }

    public async Task<CalculationResponse> SendCalculationRequestAsync(CalculationRequest request, int timeoutSeconds = 30)
    {
        try
        {
            // Ensure connection is established
            if (!_connectionService.IsConnected)
            {
                await _connectionService.ConnectAsync();
            }

            _logger.LogInformation("Sending calculation request: {Operation} {Operand1} {Operand2}, CorrelationId: {CorrelationId}",
                request.Operation, request.Operand1, request.Operand2, request.CorrelationId);

            // Send request
            await SendRequestAsync(request);

            // Wait for response
            var response = await WaitForResponseAsync(request.CorrelationId, timeoutSeconds);

            _logger.LogInformation("Received calculation response: Result={Result}, Success={Success}, CorrelationId: {CorrelationId}",
                response.Result, response.Success, response.CorrelationId);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending calculation request, CorrelationId: {CorrelationId}", request.CorrelationId);
            
            return new CalculationResponse
            {
                Success = false,
                ErrorMessage = $"Failed to send calculation request: {ex.Message}",
                CorrelationId = request.CorrelationId
            };
        }
    }

    private async Task SendRequestAsync(CalculationRequest request)
    {
        try
        {
            var requestJson = JsonSerializer.Serialize(request);
            _connectionService.SendMessage(_config.RequestQueueName, requestJson);

            _logger.LogDebug("Successfully sent request to queue {RequestQueue}, CorrelationId: {CorrelationId}",
                _config.RequestQueueName, request.CorrelationId);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send request, CorrelationId: {CorrelationId}", request.CorrelationId);
            throw;
        }
    }

    private async Task<CalculationResponse> WaitForResponseAsync(string correlationId, int timeoutSeconds)
    {
        var timeoutTime = DateTime.UtcNow.AddSeconds(timeoutSeconds);

        try
        {
            while (DateTime.UtcNow < timeoutTime)
            {
                try
                {
                    if (_connectionService.TryReceiveMessage(_config.ResponseQueueName, out string? responseJson) && !string.IsNullOrEmpty(responseJson))
                    {
                        var response = JsonSerializer.Deserialize<CalculationResponse>(responseJson);
                        if (response != null && response.CorrelationId == correlationId)
                        {
                            _logger.LogDebug("Received response from queue {ResponseQueue}, CorrelationId: {CorrelationId}",
                                _config.ResponseQueueName, correlationId);
                            return response;
                        }
                    }

                    // No matching message yet, wait a bit
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking for response, CorrelationId: {CorrelationId}", correlationId);
                    await Task.Delay(1000);
                }
            }

            // Timeout occurred
            _logger.LogWarning("Timeout waiting for response, CorrelationId: {CorrelationId}", correlationId);
            return new CalculationResponse
            {
                Success = false,
                ErrorMessage = $"Timeout waiting for response after {timeoutSeconds} seconds",
                CorrelationId = correlationId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error waiting for response, CorrelationId: {CorrelationId}", correlationId);
            return new CalculationResponse
            {
                Success = false,
                ErrorMessage = $"Error waiting for response: {ex.Message}",
                CorrelationId = correlationId
            };
        }
    }
}