using System.Text;
using System.Text.Json;
using CheckInDocMCP.Domain;
using Microsoft.Extensions.Logging;

namespace CheckInDocMCP;

public class ResearchService
{
    private readonly string apiKey = string.Empty;
    private readonly string model = string.Empty;
    private readonly string endpoint = string.Empty;
    private readonly HttpClient? httpClient;
    private readonly ILogger<ResearchService> logger;

    private readonly string[] allowedDomains = new string[] { "docs.reka.ai" };

    /// <summary>
    /// Initializes a new instance of the <see cref="ResearchService"/> class.
    /// </summary>
    /// <param name="client">The HTTP client used for making API requests.</param>
    /// <param name="apiKey">The API key for authenticating with the Reka AI service. This is sensitive information.</param>
    /// <param name="allowedDomains">An array of allowed domains for web search research.</param>
    /// <param name="logger">The logger instance for logging operations.</param>
    public ResearchService(HttpClient client, string apiKey, string[] allowedDomains, ILogger<ResearchService> logger)
    {
        this.apiKey = apiKey;
        this.allowedDomains = allowedDomains;
        this.logger = logger;

        httpClient = client;
        model = "reka-flash-research";
        endpoint = "http://api.reka.ai/v1/chat/completions";

        // Log initialization without exposing the API key
        logger.LogInformation("ResearchService initialized with model: {Model}, allowed domains: {Domains}",
            model, string.Join(", ", allowedDomains));
    }


    public async Task<string> CheckInDoc(string question)
    {

        var requestPayload = new
        {
            model = model,

            messages = new[]
            {
                new
                {
                    role = "user",
                    content = question
                }
            },

            research = new
            {
                web_search = new
                {
                    allowed_domains = allowedDomains,
                    max_uses = 4

                },
                parallel_thinking = new
                {
                    mode = "high"
                }
            },
        };

        var jsonPayload = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        HttpResponseMessage? response = null;

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            response = await httpClient!.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var rekaResponse = JsonSerializer.Deserialize<RekaResponse>(responseContent);
                var answerStr = rekaResponse!.Choices![0]!.Message!.Content;
                return answerStr!;
            }
            else
            {
                throw new Exception($"Request failed with status code: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception exp)
        {
            var exMsg = $"An error occurred while processing the request: {exp.Message}";
            Console.WriteLine(exMsg);
            return exMsg;
        }
    }
}







