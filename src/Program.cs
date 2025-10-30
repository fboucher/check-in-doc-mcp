
using CheckInDocMCP;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Env.TraversePath().Load();
string apiKey = Environment.GetEnvironmentVariable("APIKEY") ?? throw new InvalidOperationException("APIKEY is not set.");
string[] allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_DOMAINS")!.Split(",") ?? throw new InvalidOperationException("ALLOWED_DOMAINS is not set. Set it to a comma-separated list of domains.");

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ResearchService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var logger = sp.GetRequiredService<ILogger<ResearchService>>();
    return new ResearchService(httpClient, apiKey, allowedDomains, logger);
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();