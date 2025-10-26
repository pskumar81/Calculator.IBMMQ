using Calculator.Server.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();

// Add services to the container
builder.Services.AddCalculatorServerServices(builder.Configuration);

var host = builder.Build();

// Run the host
await host.RunAsync();