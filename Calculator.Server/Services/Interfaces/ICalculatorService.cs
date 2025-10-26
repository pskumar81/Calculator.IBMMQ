using Calculator.Server.Models;

namespace Calculator.Server.Services.Interfaces;

/// <summary>
/// Interface for calculator service operations
/// </summary>
public interface ICalculatorService
{
    /// <summary>
    /// Performs a calculation based on the request
    /// </summary>
    /// <param name="request">The calculation request</param>
    /// <returns>The calculation response</returns>
    Task<CalculationResponse> CalculateAsync(CalculationRequest request);
}