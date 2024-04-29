using chatbot_mock_be.Data.Enum;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_mock_be.Data;

public interface IMessage
{
    TypeEnum getType();
    String getText();
    String getUser();
}
