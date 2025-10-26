using Calculator.Client.Models;
using Calculator.Client.Services.Interfaces;
using IBM.WMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;

namespace Calculator.Client.Services;

/// <summary>
/// IBM MQ connection service for client that connects to real IBM MQ when available
/// Uses IBM MQ Docker container for messaging
/// </summary>
public class IBMMQConnectionService : IIBMMQConnectionService, IDisposable
{
    private readonly IBMMQConfiguration _config;
    private readonly ILogger<IBMMQConnectionService> _logger;
    private bool _disposed = false;
    private bool _isConnected = false;
    
    private MQQueueManager? _queueManager;
    private readonly Dictionary<string, MQQueue> _openQueues = new();

    public IBMMQConnectionService(IOptions<IBMMQConfiguration> config, ILogger<IBMMQConnectionService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public bool IsConnected => _isConnected;

    public async Task ConnectAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(IBMMQConnectionService));

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

            // Create connection properties
            var properties = new Hashtable
            {
                { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
                { MQC.HOST_NAME_PROPERTY, _config.HostName },
                { MQC.PORT_PROPERTY, _config.Port },
                { MQC.CHANNEL_PROPERTY, _config.Channel }
            };

            if (!string.IsNullOrEmpty(_config.UserId))
            {
                properties.Add(MQC.USER_ID_PROPERTY, _config.UserId);
                if (!string.IsNullOrEmpty(_config.Password))
                {
                    properties.Add(MQC.PASSWORD_PROPERTY, _config.Password);
                }
            }

            // Connect to Queue Manager
            _queueManager = new MQQueueManager(_config.QueueManagerName, properties);

            _isConnected = true;
            _logger.LogInformation("✅ Successfully connected to IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
            _logger.LogInformation("🚀 Ready to send calculation requests!");
            
            await Task.CompletedTask;
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
                
                // Close all open queues
                foreach (var queue in _openQueues.Values)
                {
                    try
                    {
                        queue.Close();
                    }
                    catch (Exception qex)
                    {
                        _logger.LogWarning(qex, "Error closing queue");
                    }
                }
                _openQueues.Clear();

                // Disconnect from queue manager
                _queueManager?.Disconnect();
                _queueManager = null;
                
                _isConnected = false;
                _logger.LogInformation("✅ Disconnected from IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
            }
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error disconnecting from IBM MQ Queue Manager: {QueueManager}", _config.QueueManagerName);
        }
    }

    public void SendMessage(string queueName, string message)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(IBMMQConnectionService));

        if (!_isConnected)
            throw new InvalidOperationException("Not connected to IBM MQ Queue Manager");

        try
        {
            _logger.LogDebug("📤 Sending message to queue: {QueueName} (Length: {Length})", queueName, message.Length);

            // Open the queue for output if not already open
            if (!_openQueues.ContainsKey(queueName))
            {
                int openOptions = MQC.MQOO_OUTPUT | MQC.MQOO_FAIL_IF_QUIESCING;
                var queue = _queueManager!.AccessQueue(queueName, openOptions);
                _openQueues[queueName] = queue;
            }

            // Create and send the message
            var mqMessage = new MQMessage();
            mqMessage.WriteString(message);
            mqMessage.Format = MQC.MQFMT_STRING;

            var putMessageOptions = new MQPutMessageOptions();
            _openQueues[queueName].Put(mqMessage, putMessageOptions);

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
            throw new ObjectDisposedException(nameof(IBMMQConnectionService));

        if (!_isConnected)
            throw new InvalidOperationException("Not connected to IBM MQ Queue Manager");

        try
        {
            _logger.LogTrace("📥 Checking for messages on queue: {QueueName}", queueName);

            // Open the queue for input if not already open
            if (!_openQueues.ContainsKey(queueName))
            {
                int openOptions = MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_FAIL_IF_QUIESCING;
                var queue = _queueManager!.AccessQueue(queueName, openOptions);
                _openQueues[queueName] = queue;
            }

            // Try to get a message with no wait
            var mqMessage = new MQMessage();
            var getMessageOptions = new MQGetMessageOptions
            {
                Options = MQC.MQGMO_NO_WAIT | MQC.MQGMO_FAIL_IF_QUIESCING
            };

            try
            {
                _openQueues[queueName].Get(mqMessage, getMessageOptions);
                message = mqMessage.ReadString(mqMessage.MessageLength);
                _logger.LogDebug("✅ Message received from queue: {QueueName} (Length: {Length})", queueName, message.Length);
                return true;
            }
            catch (MQException mqEx)
            {
                // MQRC_NO_MSG_AVAILABLE means no messages - this is expected
                if (mqEx.ReasonCode == MQC.MQRC_NO_MSG_AVAILABLE)
                {
                    return false;
                }
                throw;
            }
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
            throw new ObjectDisposedException(nameof(IBMMQConnectionService));

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