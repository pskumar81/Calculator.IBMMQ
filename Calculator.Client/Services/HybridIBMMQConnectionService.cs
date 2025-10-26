using Calculator.Client.Models;
using Calculator.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Calculator.Client.Services;

/// <summary>
/// Hybrid IBM MQ connection service for client that can use real IBM MQ when available
/// For now, it logs the real configuration but uses simulation for messaging
/// This provides a foundation for real IBM MQ integration
/// </summary>
public class HybridIBMMQConnectionService : IIBMMQConnectionService, IDisposable
{
    private readonly IBMMQConfiguration _config;
    private readonly ILogger<HybridIBMMQConnectionService> _logger;
    private bool _disposed = false;
    private bool _isConnected = false;

    // For now, we'll use the simulation queues but with real connection logic preparation
    private readonly Queue<string> _requestQueue = new();
    private readonly Queue<string> _responseQueue = new();

    public HybridIBMMQConnectionService(IOptions<IBMMQConfiguration> config, ILogger<HybridIBMMQConnectionService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public bool IsConnected => _isConnected;

    public async Task ConnectAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HybridIBMMQConnectionService));

        try
        {
            if (_isConnected)
            {
                _logger.LogDebug("Already connected to IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
                return;
            }

            _logger.LogInformation("🔌 Connecting to IBM MQ Queue Manager: {QueueManager} at {Host}:{Port} via {Channel}",
                _config.QueueManagerName, _config.HostName, _config.Port, _config.Channel);

            _logger.LogInformation("🔐 Using credentials: User={User}, SSL={UseSSL}", 
                _config.UserId ?? "None", _config.UseSSL ?? false);

            _logger.LogInformation("📋 Target queues: Request={RequestQueue}, Response={ResponseQueue}",
                _config.RequestQueueName, _config.ResponseQueueName);

            // TODO: Replace this with real IBM MQ connection using IBMMQDotnetClient
            // For now, simulate the connection
            await Task.Delay(100); // Simulate connection time

            _isConnected = true;
            _logger.LogInformation("✅ Successfully connected to IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
            _logger.LogInformation("🚀 Ready to send calculation requests!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to connect to IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_disposed)
            return;

        try
        {
            if (_isConnected)
            {
                _logger.LogInformation("📴 Disconnecting from IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
                
                // TODO: Add real IBM MQ disconnection logic here
                await Task.Delay(50); // Simulate disconnection time
                
                _isConnected = false;
                _logger.LogInformation("✅ Disconnected from IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error disconnecting from IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
        }
    }

    public void SendMessage(string queueName, string message)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HybridIBMMQConnectionService));

        if (!_isConnected)
            throw new InvalidOperationException("Not connected to IBM MQ Queue Manager");

        try
        {
            _logger.LogDebug("📤 Sending message to queue: {QueueName} (Length: {Length})", queueName, message.Length);

            // TODO: Replace with real IBM MQ message sending using IBMMQDotnetClient
            // For now, use simulation queues
            if (queueName == _config.RequestQueueName || queueName.Contains("REQUEST"))
            {
                _requestQueue.Enqueue(message);
            }
            else if (queueName == _config.ResponseQueueName || queueName.Contains("RESPONSE"))
            {
                _responseQueue.Enqueue(message);
            }

            _logger.LogDebug("✅ Message sent to queue: {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send message to queue: {QueueName}", queueName);
            throw;
        }
    }

    public bool TryReceiveMessage(string queueName, out string? message)
    {
        message = null;

        if (_disposed)
            throw new ObjectDisposedException(nameof(HybridIBMMQConnectionService));

        if (!_isConnected)
            throw new InvalidOperationException("Not connected to IBM MQ Queue Manager");

        try
        {
            _logger.LogTrace("📥 Checking for messages on queue: {QueueName}", queueName);

            // TODO: Replace with real IBM MQ message receiving using IBMMQDotnetClient
            // For now, use simulation queues
            bool hasMessage = false;
            if (queueName == _config.RequestQueueName || queueName.Contains("REQUEST"))
            {
                hasMessage = _requestQueue.TryDequeue(out message);
            }
            else if (queueName == _config.ResponseQueueName || queueName.Contains("RESPONSE"))
            {
                hasMessage = _responseQueue.TryDequeue(out message);
            }

            if (hasMessage && message != null)
            {
                _logger.LogDebug("✅ Message received from queue: {QueueName} (Length: {Length})", queueName, message.Length);
            }

            return hasMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to receive message from queue: {QueueName}", queueName);
            throw;
        }
    }

    public async Task<string?> ReceiveMessageAsync(string queueName, int timeoutMs = 5000)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HybridIBMMQConnectionService));

        if (!_isConnected)
            throw new InvalidOperationException("Not connected to IBM MQ Queue Manager");

        try
        {
            _logger.LogDebug("⏳ Waiting for message from queue: {QueueName} (timeout: {Timeout}ms)", queueName, timeoutMs);

            var startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalMilliseconds < timeoutMs)
            {
                if (TryReceiveMessage(queueName, out string? message))
                {
                    _logger.LogDebug("✅ Received message from queue: {QueueName}", queueName);
                    return message;
                }

                // Small delay to prevent busy waiting
                await Task.Delay(10);
            }

            _logger.LogDebug("⏰ Timeout waiting for message from queue: {QueueName}", queueName);
            return null; // Timeout
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to receive message from queue: {QueueName}", queueName);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _logger.LogInformation("🔄 Disposing IBM MQ connection service");
        DisconnectAsync().Wait();
        _disposed = true;
    }
}