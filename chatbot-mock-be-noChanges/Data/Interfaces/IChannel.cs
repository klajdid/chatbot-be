using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Mvc;
using StreamChat.Clients;
using StreamChat.Models;

namespace chatbot_mock_be.Data.Interfaces;


public abstract class Channel {
    
    protected readonly IConfiguration _configuration;
    protected readonly StreamClientFactory _factory;
    protected readonly  IMessageClient _messageClient;
    protected readonly string adminUser = "joni-shpk";

    protected Channel()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json");

        _configuration = builder.Build();
        _factory = new StreamClientFactory(_configuration["Configurations:ApiKey"], _configuration["Configurations:ApiSecret"]);
        _messageClient = _factory.GetMessageClient();
        _messageClient = _factory.GetMessageClient();
    }
    

    public abstract Task ReceiveMessage(MessageDto mess);
    public abstract Task<ConfigurationDto> BeginConversation (ConfigurationRequestDto configReq);
    public abstract Task DeleteChat(ConfigurationRequestDto requestDto);

    public async Task SendMessageAsync(MessageDto mess) => 
        await _messageClient.SendMessageAsync(mess.Type, mess.ChannelId, 
            new MessageRequest
        {
            Text = mess.Text
        }, adminUser);
    }