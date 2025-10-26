namespace Calculator.Client.Models;

/// <summary>
/// Represents a calculation request to be sent to the server
/// </summary>
public class CalculationRequest
{
    /// <summary>
    /// First operand for the calculation
    /// </summary>
    public double Operand1 { get; set; }

    /// <summary>
    /// Second operand for the calculation
    /// </summary>
    public double Operand2 { get; set; }

    /// <summary>
    /// The mathematical operation to perform
    /// </summary>
    public CalculationOperation Operation { get; set; }

    /// <summary>
    /// Unique identifier for tracking the request
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Queue name where the response should be sent
    /// </summary>
    public string ReplyTo { get; set; } = string.Empty;
}