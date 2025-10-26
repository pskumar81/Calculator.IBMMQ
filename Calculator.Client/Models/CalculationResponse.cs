namespace Calculator.Client.Models;

/// <summary>
/// Represents a calculation response received from the server
/// </summary>
public class CalculationResponse
{
    /// <summary>
    /// The result of the calculation
    /// </summary>
    public double Result { get; set; }

    /// <summary>
    /// Indicates whether the calculation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the calculation failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Correlation ID matching the original request
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;
}