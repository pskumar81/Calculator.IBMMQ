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

        // Check if we should use real IBM MQ or simulation
        var useRealIBMMQ = configuration.GetValue<bool>("UseRealIBMMQ", true);

        if (useRealIBMMQ)
        {
            // Register hybrid IBM MQ service (real configuration, simulated messaging for now)
            services.AddSingleton<IIBMMQConnectionService, HybridIBMMQConnectionService>();
        }
        else
        {
            // Register fully simulated IBM MQ services for testing
            services.AddSingleton<IIBMMQConnectionService, IBMMQConnectionService>();
        }

        services.AddSingleton<ICalculatorClientService, CalculatorClientService>();

        return services;
    }
}