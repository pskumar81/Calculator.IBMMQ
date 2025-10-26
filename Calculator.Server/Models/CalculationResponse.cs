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
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// The result of the calculation
    /// </summary>
    [JsonPropertyName("result")]
    public double Result { get; set; }

    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;

    /// <summary>
    /// Error message if the operation failed
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The operation that was performed
    /// </summary>
    [JsonPropertyName("operation")]
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the response was created
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    [JsonPropertyName("processingTimeMs")]
    public long ProcessingTimeMs { get; set; }
}