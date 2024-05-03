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
        await _channel.DeleteChat(configReq);
        return Ok();
    }
    
    [HttpPost("start-chat")]
    public async Task<ActionResult>  StartChat([FromBody] Message message)
    {
        return await _channel.ReceiveMessage(message);
    }
}