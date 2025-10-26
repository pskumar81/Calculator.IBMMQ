using Calculator.Client.Models;
using Calculator.Client.Services;
using Calculator.Client.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calculator.Client.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all services required for the Calculator Client
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCalculatorClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        services.Configure<IBMMQConfiguration>(configuration.GetSection("IBMMQ"));

        // Register services
        services.AddSingleton<IIBMMQConnectionService, IBMMQConnectionService>();
        services.AddSingleton<ICalculatorClientService, CalculatorClientService>();

        return services;
    }
}