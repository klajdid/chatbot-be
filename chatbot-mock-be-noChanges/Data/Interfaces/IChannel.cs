using chatbot_mock_be.Data.Enum;
using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_mock_be.Data.Interfaces;


public interface Channel {
    Task<ActionResult> ReceiveMessage(MessageDto mess);
    Task<ConfigurationDto> BeginConversation (ConfigurationRequestDto configReq);
    Task<ActionResult> DeleteChat(ConfigurationRequestDto requestDto);
}