using chatbot_mock_be.Data.Enum;

namespace chatbot_mock_be.Data;

public class Message
{
    public string Type { get; set; } 
    public  string Text { get; set; }
    public  string UserId { get; set; }
    public string ChannelId { get; set; }

    // public Message(TypeEnum type, string text, string user, ChannelID channelId)
    // {
    //     Type = type;
    //     Text = text;
    //     User = user;
    //     ChannelId = channelId;
    // }
}