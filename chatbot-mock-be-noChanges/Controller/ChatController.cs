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
    
    [HttpGet("initialize-chat")]
    public IActionResult InitializeChat()
    {
        var configData = _config.GetConfigData();
        return Ok(configData);
    }
}