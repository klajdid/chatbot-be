using chatbot_mock_be.Data;
using chatbot_mock_be.Data.Concretes;
using chatbot_mock_be.Dto;
using Microsoft.AspNetCore.Mvc;

namespace chatbot_mock_be.Controller;
[ApiController]
[Route("api")]
public class ChatController : ControllerBase
{
    private readonly Channel _channel;
    private readonly ConfigService _config;


    public ChatController(Channel channel, ConfigService config)
    {
        _channel = channel;
        _config = config;
    }

    [HttpPost("Message-Management")]
    public async Task<ActionResult> MessageManagement([FromBody] Message message)
    {
        Console.WriteLine(message.UserId);
        return await _channel.ReceiveMessage(message);
    }
    
    [HttpPost("initialize-chat")]
    public async Task<ActionResult> InitializeChat([FromBody] ConfigurationRequestDto configReq)
    {
        var configData = await _config.GetConfigData(configReq);
        return Ok(configData);
    }
    
    [HttpPost("delete-chat")]
    public async Task<ActionResult> DeleteChat([FromBody] ConfigurationRequestDto configReq)
    {
        var configData = await _channel.DeleteChat(configReq);
        return Ok(configData);
    }
    
    [HttpPost("start-chat")]
    public async Task<ActionResult>  StartChat([FromBody] Message message)
    {
        return await _channel.ReceiveMessage(message);
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