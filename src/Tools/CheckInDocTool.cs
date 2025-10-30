using System;
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace CheckInDocMCP.Tools;

[McpServerToolType]
public class CheckInDocTool(ResearchService client)
{
    private readonly ResearchService researchService = client;

    [McpServerTool, Description("Search about a topic in documentaion websites")]
    public async Task<string> SearchInDoc(string question)
    {
        var completedQuestion = $"Question: {question} \nAnswer the question based on documentation websites and provide the source URL in the answer.";
        return await researchService.CheckInDoc(question);
    }
}
