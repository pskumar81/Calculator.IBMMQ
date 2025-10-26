using Calculator.Client.Extensions;
using Calculator.Client.Models;
using Calculator.Client.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();

// Add services to the container
builder.Services.AddCalculatorClientServices(builder.Configuration);

var host = builder.Build();

// Get the calculator client service
var calculatorService = host.Services.GetRequiredService<ICalculatorClientService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("=== IBM MQ Calculator Client ===");
logger.LogInformation("This client demonstrates the IBM MQ-based calculator service.");
logger.LogInformation("Available operations: Add, Subtract, Multiply, Divide");

try
{
    while (true)
    {
        Console.WriteLine("\n=== IBM MQ Calculator Client ===");
        Console.WriteLine("Choose an operation:");
        Console.WriteLine("1. Add");
        Console.WriteLine("2. Subtract");
        Console.WriteLine("3. Multiply");
        Console.WriteLine("4. Divide");
        Console.WriteLine("5. Exit");
        Console.Write("Enter your choice (1-5): ");

        var choice = Console.ReadLine();

        if (choice == "5")
        {
            logger.LogInformation("Exiting calculator client");
            break;
        }

        if (string.IsNullOrEmpty(choice) || !int.TryParse(choice, out var operation) || operation < 1 || operation > 4)
        {
            Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
            continue;
        }

        Console.Write("Enter first number: ");
        if (!double.TryParse(Console.ReadLine(), out var operand1))
        {
            Console.WriteLine("Invalid number. Please enter a valid decimal number.");
            continue;
        }

        Console.Write("Enter second number: ");
        if (!double.TryParse(Console.ReadLine(), out var operand2))
        {
            Console.WriteLine("Invalid number. Please enter a valid decimal number.");
            continue;
        }

        CalculationResponse? result = null;

        try
        {
            Console.WriteLine("Sending request to server...");

            result = operation switch
            {
                1 => await calculatorService.AddAsync(operand1, operand2),
                2 => await calculatorService.SubtractAsync(operand1, operand2),
                3 => await calculatorService.MultiplyAsync(operand1, operand2),
                4 => await calculatorService.DivideAsync(operand1, operand2),
                _ => throw new InvalidOperationException("Invalid operation")
            };

            if (result.Success)
            {
                var operationName = operation switch
                {
                    1 => "Addition",
                    2 => "Subtraction",
                    3 => "Multiplication",
                    4 => "Division",
                    _ => "Unknown"
                };

                Console.WriteLine($"\n{operationName} Result:");
                Console.WriteLine($"{operand1} {GetOperatorSymbol(operation)} {operand2} = {result.Result}");
                logger.LogInformation("Calculation completed successfully: {Operand1} {Operator} {Operand2} = {Result}",
                    operand1, GetOperatorSymbol(operation), operand2, result.Result);
            }
            else
            {
                Console.WriteLine($"\nError: {result.ErrorMessage}");
                logger.LogError("Calculation failed: {ErrorMessage}", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
            logger.LogError(ex, "Unexpected error during calculation");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Fatal error in calculator client");
    Console.WriteLine($"Fatal error: {ex.Message}");
}
finally
{
    await host.StopAsync();
}

static string GetOperatorSymbol(int operation)
{
    return operation switch
    {
        1 => "+",
        2 => "-",
        3 => "*",
        4 => "/",
        _ => "?"
    };
}