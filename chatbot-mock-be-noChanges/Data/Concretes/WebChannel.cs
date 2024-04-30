using System.Text.RegularExpressions;
using chatbot_mock_be.Data.Enum;
using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StreamChat.Clients;
using StreamChat.Models;

namespace chatbot_mock_be.Data.Interfaces;

public class WebChannel : Channel
{
    private readonly StreamClientFactory _factory;
    private readonly IMessageClient _messageClient;
    private readonly IChannelClient _channelClient;
    private readonly string adminUser = "joni-shpk";
    private readonly ChannelRequest _chanData;
    public WebChannel()
    { 
      _factory = new StreamClientFactory("sjd3wynvqdpz", "njbw4qn9phrmhsybd6jq76y2rtcm8nu2uchr265mktanrysnancp3s3kcz4xkn8s");
      _messageClient = _factory.GetMessageClient();
      _channelClient = _factory.GetChannelClient(); // Get the Channel Client
      _chanData = new ChannelRequest { CreatedBy = new UserRequest { Id = "crimson-hat-9" } };
      var user = new UserRequest
      {
          Id = "bob-1",
          Role = Role.User,
      };
      user.SetData("prova", "datademo");

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
        if (!mess.UserId.Equals(adminUser))
        {
            var botMessage = BotMessage(mess.Text.ToLower());
            if (!string.IsNullOrEmpty(botMessage))
            {
                // var messageClient = _factory.GetMessageClient();
                var toBeSent = new MessageRequest
                {
                    Text = botMessage,
                    HTML = "<p>HELLO<p>"
                };
                
                var channelId = new ChannelID(mess.ChannelId);
                Console.WriteLine(mess.Text);
                Console.WriteLine(mess.UserId);
                
                await _messageClient.SendMessageAsync(mess.Type, mess.ChannelId, toBeSent, adminUser);
                return new OkResult();
            }
        }
        return new OkResult();
    }

    public async Task<ActionResult> DeleteChat(ConfigurationRequestDto requestDto)
    {
        if (!string.IsNullOrEmpty(requestDto.ChannelId))
            await _channelClient.DeleteAsync("messaging", requestDto.ChannelId);

        return new OkResult();
    }

    public Message SendMessage()
    {
        return new Message();
    }

    private static string lastResponse = string.Empty;

    private string BotMessage(string userMessage)
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
            case "do it on bold":
                if (!string.IsNullOrEmpty(lastResponse))
                {
                    return $"<strong>{lastResponse}</strong>";
                }
                return "No previous message to bold.";
            case "firstmessage":
                return "Hello there";
            default:
                lastResponse = string.Empty;
                break;
        }

        // If the user message contains "bold" and there's a last response, return it with bold tags
        if (makeBold && !string.IsNullOrEmpty(lastResponse))
        {
            return $"<strong>{lastResponse}</strong>";
        }

        return lastResponse;
    }

// Helper method to remove formatting tags from the user message
    private string RemoveFormatting(string text)
    {
        // Define a regular expression pattern to remove HTML tags
        string pattern = @"<[^>]+>|&nbsp;";
    
        // Replace matches with an empty string
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
    



}