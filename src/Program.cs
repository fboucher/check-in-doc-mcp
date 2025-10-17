
using CheckInDocMCP;
using CheckInDocMCP.Tools;

var builder = WebApplication.CreateBuilder(args);       
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Services.AddMcpServer()
    .WithTools<CheckInDocTool>().WithHttpTransport();

builder.Services.AddHttpClient<ResearchClient>();
            
var app = builder.Build();

app.MapMcp();

app.Run();