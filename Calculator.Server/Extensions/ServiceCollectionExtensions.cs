using Calculator.Server.Models;
using Calculator.Server.Services;
using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calculator.Server.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all services required for the Calculator Server
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCalculatorServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        services.Configure<IBMMQConfiguration>(configuration.GetSection("IBMMQ"));

        // Register core services
        services.AddSingleton<IIBMMQConnectionService, IBMMQConnectionService>();
        services.AddSingleton<ICalculatorService, CalculatorService>();
        services.AddSingleton<IIBMMQConsumerService, IBMMQConsumerService>();

        // Register background service
        services.AddHostedService<CalculatorServerBackgroundService>();

        return services;
    }
}