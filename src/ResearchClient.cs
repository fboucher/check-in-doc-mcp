using System;
using System.ClientModel;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using CheckInDocMCP.Domain;
using DotNetEnv;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

namespace CheckInDocMCP;

public class ResearchClient
{
    private string apiKey = string.Empty;
    private readonly string model = string.Empty;
    private string endpoint = string.Empty;
    private readonly HttpClient? httpClient;

    private readonly string[] allowedDomains = new string[] { "docs.reka.ai" };

    public ResearchClient(HttpClient client)
    {
        Env.TraversePath().Load();
        apiKey = Environment.GetEnvironmentVariable("APIKEY") ?? throw new InvalidOperationException("APIKEY is not set.");

        httpClient = client;
        model = "reka-flash-research";
        endpoint = "http://api.reka.ai/v1";
        allowedDomains = Environment.GetEnvironmentVariable("ALLOWED_DOMAINS")!.Split(",") ?? throw new InvalidOperationException("ALLOWED_DOMAINS is not set. Set it to a comma-separated list of domains.");
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
                }
            },
        };

        var jsonPayload = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        HttpResponseMessage? response = null;

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        response = await httpClient!.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        var rekaResponse = JsonSerializer.Deserialize<RekaResponse>(responseContent);

        if (response.IsSuccessStatusCode)
        {
            var answerStr = rekaResponse!.Choices![0]!.Message!.Content;
            return answerStr!;
        }
        else
        {
            throw new Exception($"Request failed with status code: {response.StatusCode}. Response: {responseContent}");
        }
    }
}







