using Newtonsoft.Json.Linq;
using StreamChat.Clients;
using StreamChat.Models;

namespace chatbot_mock_be.BotResponses.AssistantApiManager.AssistantApiImplementation;

public class AssistantAiService
{
    private AssistantApiClient assistantApiClient;
    private readonly IEventClient _eventClient;
    private readonly IConfiguration _configuration;
    private readonly string adminUser = "joni-shpk";

    public AssistantAiService(IEventClient eventClient, IConfiguration configuration)
    {
        _configuration = configuration;
        assistantApiClient = new AssistantApiClient(_configuration["Configurations:OpenAiKey"] ?? "");;
        _eventClient = eventClient; 
    }

    //Makes the call to the assistant openAi so that the response should be generated form openAi source.
    public async Task<string> AssistantOpenAiResponse(string userMessage, string channelId, string assistantThread)
    {
        String assistantId = _configuration["Configurations:AssistantId"] ?? "";
        string outputResult = "";
        string status = "";
        var runId = await assistantApiClient.AskAssistant(assistantThread, assistantId, userMessage);
        do
        {
            var channelEventStart = new Event { Type = "typing.start", UserId = adminUser };
            await _eventClient.SendEventAsync("messaging", channelId, channelEventStart);
            // await Task.Delay(2000);
            status = await assistantApiClient.CheckStatus(assistantThread, runId);
        }
        while (!status.Equals("completed") && !status.Equals("failed"));
        if (status.Equals("completed"))
        {
            var channelEventStop = new Event { Type = "typing.stop", UserId = adminUser };
            await _eventClient.SendEventAsync("messaging", channelId, channelEventStop);
            var result = await assistantApiClient.GetResults(assistantThread);//quando mi devo vaccinare per l'epatite?
            var jsonObject = JObject.Parse(result.ToString());

            outputResult = (string)jsonObject["data"][0]["content"][0]["text"]["value"];
            Console.WriteLine("Assistant: " + outputResult); 
        }
        else
            Console.WriteLine("Something went wrong!");
        
        return outputResult;
    }
}