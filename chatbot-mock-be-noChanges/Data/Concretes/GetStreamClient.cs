using Microsoft.AspNetCore.Mvc;
using Stream;
using StreamChat.Clients;
using StreamChat.Models;

namespace chatbot_mock_be.Data.Concretes;

public class GetStreamClient : IGetStreamClient
{
    private readonly HttpClient _httpClient;
    public GetStreamClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient = new HttpClient();

    }

    public async Task<ActionResult> CreateChannelAsync(string channelId, string channelType, string userId)
    {
        return new OkObjectResult(200);
    }

    public async Task<ActionResult> Send()
    {
        
        // var client = new StreamClient("h5y3ytpvzcyn", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiY3JpbXNvbi1oYXQtOSJ9.g1RkDXtd5F_7grvvGsD7XZvEQTLbfM-gTaaF1-kw4xs");
        // var response = await client.Users.GetAsync("crimson-hat-9");
        
        var factory = new StreamClientFactory("h5y3ytpvzcyn", "37ruxfmkmayw767wkgsxq6wj6w4fhxdjw65k3b6p6favqn9rx6w4zhqg25psyuc5");
        var messageClient = factory.GetMessageClient();
        var toBeSent = new MessageRequest
        {
            Text = "@Josh I told them I was pesca-pescatarian. Which is one who eats solely fish who eat other fish.",    
            MentionedUsers = new[] { "crimson-hat-9",  }
        };
        await messageClient.SendMessageAsync("messaging", "talking-about-angular", toBeSent, "crimson-hat-9");
        var userClient = factory.GetUserClient();
        
        // MessageRequest messag1e = new MessageRequest { Text = "Check this bear out https://imgur.com/r/bears/4zmGbMN" };
        // GenericMessageResponse response = await messageClient.SendMessageAsync("messaging", "h5y3ytpvzcyn", message, "jenny");

        // await _messageClient.SendMessageAsync("messaging",
        //     "h5y3ytpvzcyn",
        //     "jenny",
        //     "Check this bear out https://imgur.com/r/bears/4zmGbMN");
        return new OkObjectResult(200);
    }

    private ActionResult StatusCode(int statusCode, string message)
    {
        switch (statusCode)
        {
            case 200:
                return new OkObjectResult(message);
            case 400:
                return new BadRequestObjectResult(message);
            case 404:
                return new NotFoundObjectResult(message);
            case 500:
                return StatusCode(500, message);
            default:
                return new StatusCodeResult(statusCode);
        }
    }

    public Task DeleteChannelAsync(string channelId)
    {
        throw new NotImplementedException();
    }
    

    public Task CreateUserAsync(string userId, string name, string profilePictureUrl)
    {
        throw new NotImplementedException();
    }
}

internal class MessageClientFactory
{
    // public MessageClient CreateMessageClient(IRestClient client)
    // {
    //     return new MessageClient(client);
    // }
}
