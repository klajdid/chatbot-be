namespace chatbot_mock_be.Dto;

public class ConfigurationDto
{
    public string ApiKey { get; set; }
    public string ChannelId { get; set; }
    public string UserId { get; set; }
    public string ChatType { get; set; }
    public string UserToken { get; set; }
    public string? AssistantThread { get; set; }
}