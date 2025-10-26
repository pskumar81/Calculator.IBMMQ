using Calculator.Client.Models;
using Calculator.Client.Services.Interfaces;
using Calculator.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Calculator.Client.Services;

/// <summary>
/// Simulated IBM MQ connection service for demonstration purposes
/// In a real implementation, this would use actual IBM MQ client libraries
/// </summary>
public class IBMMQConnectionService : IIBMMQConnectionService
{
    private readonly IBMMQConfiguration _config;
    private readonly ILogger<IBMMQConnectionService> _logger;
    private bool _disposed = false;
    private bool _isConnected = false;

    public IBMMQConnectionService(IOptions<IBMMQConfiguration> config, ILogger<IBMMQConnectionService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public bool IsConnected => _isConnected;

    public async Task ConnectAsync()
    {
        try
        {
            if (_isConnected)
            {
                _logger.LogDebug("Already connected to simulated IBM MQ");
                return;
            }

            _logger.LogInformation("Connecting to simulated IBM MQ Queue Manager: {QueueManager} at {Host}:{Port}",
                _config.QueueManagerName, _config.HostName, _config.Port);

            // Simulate connection delay
            await Task.Delay(100);

            _isConnected = true;

            _logger.LogInformation("Successfully connected to simulated IBM MQ Queue Manager: {QueueManager}",
                _config.QueueManagerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error connecting to simulated IBM MQ");
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            if (_isConnected)
            {
                _isConnected = false;
                _logger.LogInformation("Disconnected from simulated IBM MQ Queue Manager: {QueueManager}",
                    _config.QueueManagerName);
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error disconnecting from simulated IBM MQ");
        }
    }

    public void SendMessage(string queueName, string message)
    {
        try
        {
            if (!_isConnected)
            {
                throw new InvalidOperationException("Not connected to IBM MQ Queue Manager");
            }

            _logger.LogDebug("Sending message to queue: {QueueName}", queueName);

            if (queueName == _config.RequestQueueName)
            {
                SimulatedIBMMQQueues.RequestQueue.Enqueue(message);
            }
            else if (queueName == _config.ResponseQueueName)
            {
                SimulatedIBMMQQueues.ResponseQueue.Enqueue(message);
            }
            else
            {
                throw new InvalidOperationException($"Unknown queue: {queueName}");
            }

            _logger.LogDebug("Successfully sent message to queue: {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to queue {QueueName}", queueName);
            throw;
        }
    }

    public bool TryReceiveMessage(string queueName, out string? message)
    {
        message = null;

        try
        {
            if (!_isConnected)
            {
                throw new InvalidOperationException("Not connected to IBM MQ Queue Manager");
            }

            if (queueName == _config.RequestQueueName)
            {
                return SimulatedIBMMQQueues.RequestQueue.TryDequeue(out message);
            }
            else if (queueName == _config.ResponseQueueName)
            {
                return SimulatedIBMMQQueues.ResponseQueue.TryDequeue(out message);
            }
            else
            {
                throw new InvalidOperationException($"Unknown queue: {queueName}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to receive message from queue {QueueName}", queueName);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            DisconnectAsync().Wait(TimeSpan.FromSeconds(5));
            _logger.LogInformation("IBM MQ connection service disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing IBM MQ connection service");
        }
        finally
        {
            _disposed = true;
        }
    }
}