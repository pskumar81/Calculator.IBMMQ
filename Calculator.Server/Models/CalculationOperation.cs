using System.Text.Json.Serialization;

namespace Calculator.Server.Models;

/// <summary>
/// Enumeration of supported calculator operations
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CalculationOperation
{
    /// <summary>
    /// Addition operation
    /// </summary>
    Add,

    /// <summary>
    /// Subtraction operation
    /// </summary>
    Subtract,

    /// <summary>
    /// Multiplication operation
    /// </summary>
    Multiply,

    /// <summary>
    /// Division operation
    /// </summary>
    Divide
}

/// <summary>
/// Extension methods for CalculationOperation enum
/// </summary>
public static class CalculationOperationExtensions
{
    /// <summary>
    /// Converts string to CalculationOperation enum
    /// </summary>
    /// <param name="operation">Operation string</param>
    /// <returns>CalculationOperation enum value</returns>
    /// <exception cref="ArgumentException">Thrown when operation is not supported</exception>
    public static CalculationOperation ToOperation(this string operation)
    {
        return operation.ToLowerInvariant() switch
        {
            "add" => CalculationOperation.Add,
            "subtract" => CalculationOperation.Subtract,
            "multiply" => CalculationOperation.Multiply,
            "divide" => CalculationOperation.Divide,
            _ => throw new ArgumentException($"Unsupported operation: {operation}")
        };
    }

    /// <summary>
    /// Gets the string representation of the operation
    /// </summary>
    /// <param name="operation">CalculationOperation enum</param>
    /// <returns>String representation</returns>
    public static string ToStringValue(this CalculationOperation operation)
    {
        return operation.ToString();
    }

    /// <summary>
    /// Gets the mathematical symbol for the operation
    /// </summary>
    /// <param name="operation">CalculationOperation enum</param>
    /// <returns>Mathematical symbol</returns>
    public static string ToSymbol(this CalculationOperation operation)
    {
        return operation switch
        {
            CalculationOperation.Add => "+",
            CalculationOperation.Subtract => "-",
            CalculationOperation.Multiply => "*",
            CalculationOperation.Divide => "/",
            _ => "?"
        };
    }
}