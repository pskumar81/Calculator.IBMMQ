using Calculator.Server.Models;
using Calculator.Server.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Calculator.Tests;

/// <summary>
/// Unit tests for the CalculatorService
/// </summary>
public class CalculatorServiceTests
{
    private readonly Mock<ILogger<CalculatorService>> _mockLogger;
    private readonly CalculatorService _calculatorService;

    public CalculatorServiceTests()
    {
        _mockLogger = new Mock<ILogger<CalculatorService>>();
        _calculatorService = new CalculatorService(_mockLogger.Object);
    }

    [Fact]
    public async Task CalculateAsync_Add_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 10,
            Operand2 = 5,
            Operation = CalculationOperation.Add,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(15, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_Subtract_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 10,
            Operand2 = 3,
            Operation = CalculationOperation.Subtract,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(7, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_Multiply_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 4,
            Operand2 = 6,
            Operation = CalculationOperation.Multiply,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(24, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_Divide_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 15,
            Operand2 = 3,
            Operation = CalculationOperation.Divide,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(5, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_DivideByZero_ReturnsError()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 10,
            Operand2 = 0,
            Operation = CalculationOperation.Divide,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Equal("Division by zero is not allowed", result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_AddWithNegativeNumbers_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = -5,
            Operand2 = 3,
            Operation = CalculationOperation.Add,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(-2, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_MultiplyWithDecimals_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 2.5,
            Operand2 = 4.2,
            Operation = CalculationOperation.Multiply,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(10.5, result.Result, 10); // 10 decimal places precision
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_SubtractLargerFromSmaller_ReturnsNegativeResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 3,
            Operand2 = 8,
            Operation = CalculationOperation.Subtract,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(-5, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_DivideWithDecimals_ReturnsCorrectResult()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 7.5,
            Operand2 = 2.5,
            Operation = CalculationOperation.Divide,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_AddZeros_ReturnsZero()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 0,
            Operand2 = 0,
            Operation = CalculationOperation.Add,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_MultiplyByZero_ReturnsZero()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 42,
            Operand2 = 0,
            Operation = CalculationOperation.Multiply,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task CalculateAsync_DivideZeroByNumber_ReturnsZero()
    {
        // Arrange
        var request = new CalculationRequest
        {
            Operand1 = 0,
            Operand2 = 5,
            Operation = CalculationOperation.Divide,
            CorrelationId = "test-correlation-id"
        };

        // Act
        var result = await _calculatorService.CalculateAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.Result);
        Assert.Equal("test-correlation-id", result.CorrelationId);
        Assert.Null(result.ErrorMessage);
    }
}