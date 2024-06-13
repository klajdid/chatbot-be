using chatbot_mock_be.Data;
using chatbot_mock_be.Data.Interfaces;
using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_mock_be.Controller;
[ApiController]
[Route("api")]
public class ChatController : ControllerBase
{
    private readonly Channel _channel;

    public ChatController(Channel channel)
    {
        _channel = channel;
    }

    [HttpPost("Message-Management")]
    public async Task MessageManagement([FromBody] MessageDto messageDto)
    { 
        await _channel.ReceiveMessage(messageDto);
    }
    
    [HttpPost("initialize-chat")]
    public async Task<ActionResult> InitializeChat([FromBody] ConfigurationRequestDto configReq)
    {
        var configData = await _channel.BeginConversation(configReq);
        return Ok(configData);
    }
    
    [HttpPost("delete-chat")]
    public async Task<ActionResult> DeleteChat([FromBody] ConfigurationRequestDto configReq)
    {
        await _channel.DeleteChat(configReq);
        return Ok();
    }
    
    [HttpPost("start-chat")]
    public async Task StartChat([FromBody] MessageDto messageDto)
    {
         await _channel.ReceiveMessage(messageDto);
    }
    
    [HttpPost("/webhooks/getstream")]
    public IActionResult HandleGetStreamEvent([FromBody] GetStreamEvent request)
    {
        if (request.Type == "message_click") 
        {
            // Handle message click event
            string messageId = request.MessageId;
            Console.WriteLine(messageId);
        }
        return Ok();
    }
    public class GetStreamEvent
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
    }
}