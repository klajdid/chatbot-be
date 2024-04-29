using Microsoft.AspNetCore.Mvc;

namespace chatbot_mock_be.Data;

public interface IGetStreamClient
{
    Task<ActionResult> CreateChannelAsync(string channelId, string channelType, string userId = null);
    Task<ActionResult> Send();
    Task DeleteChannelAsync(string channelId);
    Task CreateUserAsync(string userId, string name, string profilePictureUrl);
}