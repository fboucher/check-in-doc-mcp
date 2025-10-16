using System;
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace CheckInDocMCP.Tools;

[McpServerToolType]
public class CheckInDocTool(ResearchClient client)
{
    private readonly ResearchClient researchClient = client;

    [McpServerTool, Description("Search about a topic in documentaion websites")]
    public async Task<string> SearchInDoc(string question)
    {
        return await researchClient.CheckInDoc(question);
    }
}
