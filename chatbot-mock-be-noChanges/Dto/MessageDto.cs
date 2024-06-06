namespace chatbot_mock_be.Dto;

public class MessageDto
{
    public string Type { get; set; } 
    public  string Text { get; set; }
    public  string UserId { get; set; }
    public string ChannelId { get; set; }
    public string assistantThread { get; set; }
}