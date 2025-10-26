namespace Calculator.Server.Models;

/// <summary>
/// Configuration settings for IBM MQ connection and queues
/// </summary>
public class IBMMQConfiguration
{
    /// <summary>
    /// IBM MQ server hostname
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// IBM MQ server port
    /// </summary>
    public int Port { get; set; } = 1414;

    /// <summary>
    /// Queue manager name
    /// </summary>
    public string QueueManager { get; set; } = "QM1";

    /// <summary>
    /// Channel name for connection
    /// </summary>
    public string Channel { get; set; } = "DEV.APP.SVRCONN";

    /// <summary>
    /// Username for IBM MQ authentication (optional)
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password for IBM MQ authentication (optional)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Name of the request queue
    /// </summary>
    public string RequestQueueName { get; set; } = "DEV.QUEUE.1";

    /// <summary>
    /// Name of the response queue prefix (will be appended with client ID)
    /// </summary>
    public string ResponseQueuePrefix { get; set; } = "DEV.QUEUE.RESPONSE";

    /// <summary>
    /// Request timeout in milliseconds
    /// </summary>
    public int RequestTimeoutMs { get; set; } = 30000; // 30 seconds

    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    public int ConnectionTimeoutMs { get; set; } = 5000; // 5 seconds

    /// <summary>
    /// Whether to use SSL/TLS connection
    /// </summary>
    public bool UseSSL { get; set; } = false;

    /// <summary>
    /// SSL cipher suite (if using SSL)
    /// </summary>
    public string? SSLCipherSuite { get; set; }
}