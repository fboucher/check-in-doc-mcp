using System;
using System.ClientModel;
using System.Reflection.Metadata.Ecma335;
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
    private AIAgent? agent;

    public ResearchClient()
    {
        Env.TraversePath().Load();
        apiKey = Environment.GetEnvironmentVariable("APIKEY") ?? throw new InvalidOperationException("APIKEY is not set.");
        model = "reka-flash-research";
        endpoint = "http://api.reka.ai/v1";

        SetAgent();
    }

    private void SetAgent()
    {
        ChatClientAgentOptions agentOptions = new(instructions: "You are good at telling jokes.", name: "Joker")
        {
            ChatOptions = new()
            {
                //ResponseFormat = ChatResponseFormat.ForJsonSchema<AnwerInfos>()
                
            }
        };

        AIAgent agent = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
                                .GetChatClient(model)
                                // .CreateAIAgent(agentOptions);
                                .CreateAIAgent();


    }

    public async Task<string> CheckInDoc(string question)
    {
        if (agent is null)
        {
            throw new Exception("Error in CheckInDoc, the agent is null");
        }

        var response = await agent.RunAsync(question);
        return response.Text;
    }
}







