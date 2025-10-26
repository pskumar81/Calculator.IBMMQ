namespace Calculator.Client.Services.Interfaces;

/// <summary>
/// Interface for IBM MQ connection service (Simulated for demo)
/// </summary>
public interface IIBMMQConnectionService : IDisposable
{
    /// <summary>
    /// Establishes connection to IBM MQ Queue Manager
    /// </summary>
    Task ConnectAsync();

    /// <summary>
    /// Disconnects from IBM MQ Queue Manager
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Sends a message to the specified queue
    /// </summary>
    /// <param name="queueName">Name of the queue</param>
    /// <param name="message">Message to send</param>
    void SendMessage(string queueName, string message);

    /// <summary>
    /// Tries to receive a message from the specified queue
    /// </summary>
    /// <param name="queueName">Name of the queue</param>
    /// <param name="message">Received message</param>
    /// <returns>True if message was received, false otherwise</returns>
    bool TryReceiveMessage(string queueName, out string? message);

    /// <summary>
    /// Checks if the connection is currently active
    /// </summary>
    bool IsConnected { get; }
}