using System.Text;
using System.Text.RegularExpressions;
using chatbot_mock_be.Data.Enum;
using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;
using StreamChat.Clients;
using StreamChat.Models;

namespace chatbot_mock_be.Data.Interfaces;

public class WebChannel : Channel
{
    private readonly StreamClientFactory _factory;
    private readonly IMessageClient _messageClient;
    private readonly IChannelClient _channelClient;
    private readonly IUserClient _userClient;
    private readonly IConfiguration _configuration;
    private readonly IEventClient _eventClient;
    private readonly string adminUser = "joni-shpk";
    string _openAiApiKey;
    readonly string _endpoint = "https://api.openai.com/v1/completions";

    
    public WebChannel(IConfiguration configuration)
    {
        _configuration = configuration;
      _factory = new StreamClientFactory(_configuration["Configurations:ApiKey"], _configuration["Configurations:ApiSecret"]);
      _messageClient = _factory.GetMessageClient();
      _channelClient = _factory.GetChannelClient(); // Get the Channel Client
      _userClient = _factory.GetUserClient(); // Get the User Client
      _eventClient = _factory.GetEventClient(); // Get the Event Client
      _openAiApiKey = _configuration["Configurations:OpenAiKey"] ?? "";
    }
    public ChannelID GetID()
    {
        throw new NotImplementedException();
    }

    public TypeEnum GetType()
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> ReceiveMessage(Message mess)
    {
        if (mess.UserId != adminUser)
        {
            // var botMessage = BotMessage(mess.Text.ToLower());
            var botMessage = await OpenAi(mess.Text.ToLower());;
            if (!string.IsNullOrEmpty(botMessage))
            {
                    // Sending a string message
                    var toBeSent = new MessageRequest
                    {
                        Text = (string)botMessage
                    };
                    
                    
                        var channelEventStart = new Event { Type = "typing.start", UserId = adminUser };
                        var channelEventStop = new Event { Type = "typing.stop", UserId = adminUser };
                        await _eventClient.SendEventAsync("messaging", mess.ChannelId, channelEventStart);
                        // await Task.Delay(2000);
                        await _eventClient.SendEventAsync("messaging", mess.ChannelId, channelEventStop);

                        await _messageClient.SendMessageAsync(mess.Type, mess.ChannelId, toBeSent, adminUser);
                // else if (botMessage is IEnumerable<string>)
                // {
                    // // Sending a list of URLs
                    // var urls = (IEnumerable<string>)botMessage;
                    // foreach (var url in urls)
                    // {
                    //     var toBeSent = new MessageRequest
                    //     {
                    //         Text = url
                    //     };
                    //     await Task.Delay(2000);
                    //     await _messageClient.SendMessageAsync(mess.Type, mess.ChannelId, toBeSent, adminUser);
                    // }
                // }
            }
        }
        return new OkObjectResult(200);
    }

    public async Task<ActionResult> DeleteChat(ConfigurationRequestDto requestDto)
    {
        if (!string.IsNullOrEmpty(requestDto.UserId))
            await _userClient.DeleteAsync(requestDto.UserId, markMessagesDeleted: true, hardDelete: true, deleteConversations: true);

        if (!string.IsNullOrEmpty(requestDto.ChannelId))
            await _channelClient.DeleteAsync("messaging", requestDto.ChannelId);

        return new OkObjectResult(200);
    }

    public Message SendMessage()
    {
        return new Message();
    }

    private static string lastResponse = string.Empty;

    private Object BotMessage(string userMessage)
    {
        // Check if the user message requests the response to be in bold
        bool makeBold = userMessage.ToLower().Contains("bold");

        // Remove any formatting tags from the user message
        string cleanUserMessage = RemoveFormatting(userMessage);

        switch (cleanUserMessage)
        {
            case "hello":
                lastResponse = "Hello, how can I assist you?";
                break;
            case "my head hurts":
                lastResponse = "I'm sorry to hear that your head hurts. Headaches can be really uncomfortable. Have you tried anything to relieve it, like drinking water, resting in a quiet and dark room, or maybe taking some pain relief medication? If it persists or gets worse, it might be a good idea to consult a healthcare professional.";
                break;
            case "show me the list of hospitals test":
                return new List<string>() { "https://al.spitaliamerikan.com/en/","https://al.spitaliamerikan.com/en/" };
            case "show me the list of hospitals":
                return $"<ul><li><a href=\"https://salus.al/?lang=en\" target=\"_blank\">Salus</a></li><li><a href=\"https://hygeia.al/\" target=\"_blank\">Hygeia</a></li><li><a href=\"https://al.spitaliamerikan.com/en/\" target=\"_blank\">American</a></li></ul>";
            case "do it on bold":
                if (!string.IsNullOrEmpty(lastResponse))
                {
                    return $"<strong>{lastResponse}</strong>";
                }
                return "No previous message to bold.";
            case "firstmessage":
                return "Hello there from DATAWIZ";
            default:
                lastResponse = string.Empty;
                break;
        } 
        if (makeBold && !string.IsNullOrEmpty(lastResponse))
        {
            return $"<strong>{lastResponse}</strong>";
        }

        return lastResponse;
    }

    private string RemoveFormatting(string text)
    {
        string pattern = @"<[^>]+>|&nbsp;";
        string cleanedText = Regex.Replace(text, pattern, "");
        return cleanedText;
    }
    public async Task<ActionResult> GetOrCreateChannelAsync(string channelType, string channelId)
    {
        try
        {
            var channel = await _channelClient.GetOrCreateAsync(channelType, channelId, new ChannelGetRequest());
            return new OkObjectResult(channel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing or creating the channel: {ex.Message}");
            return new BadRequestObjectResult("Failed to access or create the channel");
        }
    }

    private async Task<string> OpenAi(string userMessage)
    {
        if (userMessage.Equals("firstmessage"))
        {
            return "Hello there from DATAWIZ";
        }
        string outputResult = "";
        var openai = new OpenAIAPI(_openAiApiKey);
        CompletionRequest completionRequest = new CompletionRequest();
        completionRequest.Prompt = userMessage;
        completionRequest.MaxTokens = 1024;

        var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

        foreach (var completion in completions.Completions)
        {
            outputResult += completion.Text;
        }
        return outputResult;
    }
}