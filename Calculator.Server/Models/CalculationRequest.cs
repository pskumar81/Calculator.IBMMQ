using System.Text.Json.Serialization;

namespace Calculator.Server.Models;

/// <summary>
/// Represents a calculation request message for IBM MQ communication.
/// </summary>
public class CalculationRequest
{
    /// <summary>
    /// Unique identifier for correlating request with response in IBM MQ
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// The operation to perform
    /// </summary>
    public CalculationOperation Operation { get; set; }

    /// <summary>
    /// The first number for the calculation
    /// </summary>
    public double Operand1 { get; set; }

    /// <summary>
    /// The second number for the calculation
    /// </summary>
    public double Operand2 { get; set; }

    /// <summary>
    /// The queue name where the response should be sent
    /// </summary>
    public string ReplyTo { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the request was created
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}