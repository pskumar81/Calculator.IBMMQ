using System.Text.Json.Serialization;

namespace Calculator.Server.Models;

/// <summary>
/// Represents a calculation response message for IBM MQ communication.
/// </summary>
public class CalculationResponse
{
    /// <summary>
    /// Correlation ID matching the original request
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// The result of the calculation
    /// </summary>
    public double Result { get; set; }

    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The operation that was performed
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the response was created
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }
}