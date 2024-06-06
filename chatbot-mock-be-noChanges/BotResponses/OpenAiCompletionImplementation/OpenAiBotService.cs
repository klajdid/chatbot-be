using OpenAI_API;
using OpenAI_API.Completions;

namespace chatbot_mock_be.BotResponses.OpenAiCompletionImplementation;

public class OpenAiBotService
{
    public OpenAiBotService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private readonly IConfiguration _configuration;
    
    
    public async Task<string> CompletionOpenAiResponse(string userMessage)
    {
        if (userMessage.Equals("firstmessage"))
        {
            return "Hello there from DATAWIZ";
        }
        string outputResult = "";
        var openai = new OpenAIAPI(_configuration["Configurations:OpenAiKey"] ?? "");
        CompletionRequest completionRequest = new CompletionRequest();
        completionRequest.Prompt = userMessage;
        completionRequest.MaxTokens = 1024;
        var completions = await openai.Completions.CreateCompletionAsync(completionRequest);
        foreach (var completion in completions.Completions)
            outputResult += completion.Text;
        
        Console.WriteLine("Completions: "+ outputResult);
        return outputResult;
    }
}