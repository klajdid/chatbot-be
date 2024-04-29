using chatbot_mock_be.Data.Enum;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_mock_be.Data;


public interface Channel {
    ChannelID GetID();
    TypeEnum GetType();
    Task<ActionResult> ReceiveMessage(Message mess);
    Message SendMessage();
}

public class ChannelID {
    String id;

    public ChannelID(String id) {
        this.id = id;
    }

    public String getID() {
        return id;
    }
}