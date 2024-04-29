using chatbot_mock_be.Dto;

namespace chatbot_mock_be.Data.Concretes;

public class ConfigService
{
    private readonly IConfiguration _configuration;

    public ConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ConfigurationDto GetConfigData()
    {
        return new ConfigurationDto
        {
            ApiKey = _configuration["Configurations:ApiKey"] ?? "",
            ChannelId = _configuration["Configurations:ChannelId"] ?? "",
            UserId = _configuration["Configurations:UserId"] ?? "",
            ChatType = _configuration["Configurations:ChatType"] ?? "",
            UserToken = _configuration["Configurations:UserToken"] ?? ""
        };
    }
}