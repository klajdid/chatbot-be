using chatbot_mock_be.BotResponses.AssistantApiManager;
using chatbot_mock_be.BotResponses.AssistantApiManager.AssistantApiImplementation;
using chatbot_mock_be.BotResponses.OpenAiCompletionImplementation;
using chatbot_mock_be.Data.Enum;
using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Mvc;
using StreamChat.Clients;
using StreamChat.Models;
using Channel = chatbot_mock_be.Data.Interfaces.Channel;

namespace chatbot_mock_be.Data.Concretes;

public class WebChannel : Channel
{
    private readonly StreamClientFactory _factory;
    private readonly IMessageClient _messageClient;
    private readonly IChannelClient _channelClient;
    private readonly IUserClient _userClient;
    private readonly IConfiguration _configuration;
    private readonly IEventClient _eventClient;
    private readonly string adminUser = "joni-shpk";
    private AssistantApiClient assistantApiClient;
    string _openAiApiKey;
    private bool useAssistant = true;
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
      assistantApiClient = new AssistantApiClient(_openAiApiKey);
    }
    public TypeEnum GetType() =>
        throw new NotImplementedException();
    
    //Receives a message and handles the response that will be sent to chat which will be shown in the UI.
    //Requires a Message object as parameter with Type, Text, UserId, ChannelId properties.
    public async Task<ActionResult> ReceiveMessage(MessageDto mess)
    {
        var assistantService = new AssistantAiService(_eventClient, _configuration);
        var openAiService = new OpenAiBotService(_configuration);
        if (mess.UserId != adminUser)
        {
            var botMessage = useAssistant ?
                    await assistantService.AssistantOpenAiResponse(mess.Text.ToLower(), mess.ChannelId, mess.assistantThread)
                : await openAiService.CompletionOpenAiResponse(mess.Text.ToLower());
            if (!string.IsNullOrEmpty(botMessage))
            {
                    // Sending a string message
                    var toBeSent = new MessageRequest
                    {
                        Text = (string)botMessage
                    };
                await _messageClient.SendMessageAsync(mess.Type, mess.ChannelId, toBeSent, adminUser);
            }
        }
        return new OkObjectResult(200);
    }

    public async Task<ConfigurationDto> BeginConversation(ConfigurationRequestDto configReq)
    {
        ConfigurationDto configurationDto;
        ChannelRequest chanData;
        string token;
        string userId;
        string channelId;
        if (!string.IsNullOrEmpty(configReq.UserId) && !string.IsNullOrEmpty(configReq.ChannelId))
        {
            chanData = new ChannelRequest { CreatedBy = new UserRequest { Id = configReq.UserId } };
            await _channelClient.GetOrCreateAsync("messaging", configReq.ChannelId, new ChannelGetRequest
            {
                Data = chanData,
            });
            token = _userClient.CreateToken(configReq.UserId);
            configurationDto = new ConfigurationDto
            {
                ApiKey = _configuration["Configurations:ApiKey"] ?? "",
                ChannelId = configReq.ChannelId,
                UserId = configReq.UserId,
                ChatType = "messaging",
                UserToken = token,
                AssistantThread = configReq.AssistantThread
            };
        }
        else
        {
            Guid userGuid = Guid.NewGuid();
            Guid channelGuid = Guid.NewGuid();
            userId = userGuid.ToString();
            channelId = channelGuid.ToString();
            chanData = new ChannelRequest { CreatedBy = new UserRequest { Id = userId } };
            await _channelClient.GetOrCreateAsync("messaging", userId, new ChannelGetRequest
            {
                Data = chanData,
            });
            token = _userClient.CreateToken(userId);
            var assistantThread = await assistantApiClient.CreateThread(_configuration["Configurations:ApiKey"] ?? "");
            configurationDto = new ConfigurationDto
            {
                ApiKey = _configuration["Configurations:ApiKey"] ?? "",
                ChannelId = channelId,
                UserId = userId,
                ChatType = "messaging",
                UserToken = token,
                AssistantThread = assistantThread
            };
        }

        return configurationDto;
    }

    //Delete channel and user in the chat.
    //Requires a ConfigurationDto with UserId, ChannelId properties.
    //When the chat closes there should not be any data for this specific chat so it should be deleted.
    public async Task<ActionResult> DeleteChat(ConfigurationRequestDto requestDto)
    {
        if (!string.IsNullOrEmpty(requestDto.UserId))
            await _userClient.DeleteAsync(requestDto.UserId, markMessagesDeleted: true, hardDelete: true, deleteConversations: true);

        if (!string.IsNullOrEmpty(requestDto.ChannelId))
            await _channelClient.DeleteAsync("messaging", requestDto.ChannelId);

        return new OkObjectResult(200);
    }

    public MessageDto SendMessage() => new MessageDto();

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