using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StreamChat.Clients;
using StreamChat.Models;

namespace chatbot_mock_be.Data.Concretes;

public class ConfigService
{
    private readonly IConfiguration _configuration;
    private readonly StreamClientFactory _factory;
    private readonly IMessageClient _messageClient;
    private readonly IChannelClient _channelClient;
    private readonly IUserClient _userClient;

    public ConfigService(IConfiguration configuration)
    {
        _factory = new StreamClientFactory("sjd3wynvqdpz",
            "njbw4qn9phrmhsybd6jq76y2rtcm8nu2uchr265mktanrysnancp3s3kcz4xkn8s");
        _messageClient = _factory.GetMessageClient();
        _channelClient = _factory.GetChannelClient(); // Get the Channel Client
        _userClient = _factory.GetUserClient(); // Get the Channel Client
        _configuration = configuration;
    }

    public async Task<ConfigurationDto> GetConfigData(ConfigurationRequestDto configReq)
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
                UserToken = token
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
            configurationDto = new ConfigurationDto
            {
                ApiKey = _configuration["Configurations:ApiKey"] ?? "",
                ChannelId = channelId,
                UserId = userId,
                ChatType = "messaging",
                UserToken = token
            };
        }

        return configurationDto;
    }
}