namespace Calculator.Client.Models;

/// <summary>
/// Configuration settings for IBM MQ connection
/// </summary>
public class IBMMQConfiguration
{
    /// <summary>
    /// The name of the IBM MQ Queue Manager
    /// </summary>
    public string QueueManagerName { get; set; } = string.Empty;

    /// <summary>
    /// The hostname or IP address of the IBM MQ server
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// The port number for IBM MQ connection
    /// </summary>
    public int Port { get; set; } = 1414;

    /// <summary>
    /// The channel name for IBM MQ connection
    /// </summary>
    public string Channel { get; set; } = string.Empty;

    /// <summary>
    /// User ID for IBM MQ authentication (optional)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Password for IBM MQ authentication (optional)
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Name of the queue for sending calculation requests
    /// </summary>
    public string RequestQueueName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the queue for receiving calculation responses
    /// </summary>
    public string ResponseQueueName { get; set; } = string.Empty;

    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30000;

    /// <summary>
    /// Whether to use SSL/TLS connection
    /// </summary>
    public bool? UseSSL { get; set; } = false;

    /// <summary>
    /// Application name for identification
    /// </summary>
    public string ApplicationName { get; set; } = string.Empty;
}