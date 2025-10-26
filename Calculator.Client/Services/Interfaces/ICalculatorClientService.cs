using Calculator.Client.Models;

namespace Calculator.Client.Services.Interfaces;

/// <summary>
/// Interface for the calculator client service
/// </summary>
public interface ICalculatorClientService
{
    /// <summary>
    /// Sends a calculation request to the server and waits for the response
    /// </summary>
    /// <param name="request">The calculation request</param>
    /// <param name="timeoutSeconds">Timeout in seconds to wait for response</param>
    /// <returns>The calculation response</returns>
    Task<CalculationResponse> SendCalculationRequestAsync(CalculationRequest request, int timeoutSeconds = 30);

    /// <summary>
    /// Performs an addition operation
    /// </summary>
    /// <param name="operand1">First operand</param>
    /// <param name="operand2">Second operand</param>
    /// <returns>The calculation response</returns>
    Task<CalculationResponse> AddAsync(double operand1, double operand2);

    /// <summary>
    /// Performs a subtraction operation
    /// </summary>
    /// <param name="operand1">First operand</param>
    /// <param name="operand2">Second operand</param>
    /// <returns>The calculation response</returns>
    Task<CalculationResponse> SubtractAsync(double operand1, double operand2);

    /// <summary>
    /// Performs a multiplication operation
    /// </summary>
    /// <param name="operand1">First operand</param>
    /// <param name="operand2">Second operand</param>
    /// <returns>The calculation response</returns>
    Task<CalculationResponse> MultiplyAsync(double operand1, double operand2);

    /// <summary>
    /// Performs a division operation
    /// </summary>
    /// <param name="operand1">First operand</param>
    /// <param name="operand2">Second operand</param>
    /// <returns>The calculation response</returns>
    Task<CalculationResponse> DivideAsync(double operand1, double operand2);
}