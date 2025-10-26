using Calculator.Server.Models;
using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Calculator.Server.Services;

/// <summary>
/// Service for performing mathematical calculations
/// This contains the same business logic as the RabbitMQ version
/// </summary>
public class CalculatorService : ICalculatorService
{
    private readonly ILogger<CalculatorService> _logger;

    public CalculatorService(ILogger<CalculatorService> logger)
    {
        _logger = logger;
    }

    public async Task<CalculationResponse> CalculateAsync(CalculationRequest request)
    {
        try
        {
            _logger.LogInformation("Processing calculation: {Operation} {Operand1} {Operand2}, CorrelationId: {CorrelationId}",
                request.Operation, request.Operand1, request.Operand2, request.CorrelationId);

            var result = request.Operation switch
            {
                CalculationOperation.Add => request.Operand1 + request.Operand2,
                CalculationOperation.Subtract => request.Operand1 - request.Operand2,
                CalculationOperation.Multiply => request.Operand1 * request.Operand2,
                CalculationOperation.Divide => PerformDivision(request.Operand1, request.Operand2),
                _ => throw new InvalidOperationException($"Unsupported operation: {request.Operation}")
            };

            var response = new CalculationResponse
            {
                Result = result,
                Success = true,
                CorrelationId = request.CorrelationId
            };

            _logger.LogInformation("Calculation completed: {Operation} {Operand1} {Operand2} = {Result}, CorrelationId: {CorrelationId}",
                request.Operation, request.Operand1, request.Operand2, result, request.CorrelationId);

            return await Task.FromResult(response);
        }
        catch (DivideByZeroException)
        {
            _logger.LogWarning("Division by zero attempted, CorrelationId: {CorrelationId}", request.CorrelationId);
            
            return new CalculationResponse
            {
                Result = 0,
                Success = false,
                ErrorMessage = "Division by zero is not allowed",
                CorrelationId = request.CorrelationId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing calculation, CorrelationId: {CorrelationId}", request.CorrelationId);
            
            return new CalculationResponse
            {
                Result = 0,
                Success = false,
                ErrorMessage = $"Calculation error: {ex.Message}",
                CorrelationId = request.CorrelationId
            };
        }
    }

    private static double PerformDivision(double dividend, double divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException();
        }

        return dividend / divisor;
    }
}