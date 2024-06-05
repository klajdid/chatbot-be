using chatbot_mock_be.Utils;

namespace chatbot_mock_be.BotResponses.StaticResponseImplementation;

public class StaticResponseService
{
    private static string lastResponse = string.Empty;

    private Object BotMessage(string userMessage)
    {
        // Check if the user message requests the response to be in bold
        bool makeBold = userMessage.ToLower().Contains("bold");

        // Remove any formatting tags from the user message
        string cleanUserMessage = UserInputFormatter.RemoveFormatting(userMessage);

        //Handles the static bot message returning a specific response in base of the message that the user has entered.
        switch (cleanUserMessage)
        {
            case "hello":
                lastResponse = "Hello, how can I assist you?";
                break;
            case "my head hurts":
                lastResponse = "I'm sorry to hear that your head hurts. Headaches can be really uncomfortable. Have you tried anything to relieve it, like drinking water, resting in a quiet and dark room, or maybe taking some pain relief medication? If it persists or gets worse, it might be a good idea to consult a healthcare professional.";
                break;
            case "show me the list of hospitals test":
                return new List<string>() { "https://al.spitaliamerikan.com/en/","https://al.spitaliamerikan.com/en/" };
            case "show me the list of hospitals":
                return $"<ul><li><a href=\"https://salus.al/?lang=en\" target=\"_blank\">Salus</a></li><li><a href=\"https://hygeia.al/\" target=\"_blank\">Hygeia</a></li><li><a href=\"https://al.spitaliamerikan.com/en/\" target=\"_blank\">American</a></li></ul>";
            case "do it on bold":
                if (!string.IsNullOrEmpty(lastResponse))
                {
                    return $"<strong>{lastResponse}</strong>";
                }
                return "No previous message to bold.";
            case "firstmessage":
                return "Hello there from DATAWIZ";
            default:
                lastResponse = string.Empty;
                break;
        } 
        if (makeBold && !string.IsNullOrEmpty(lastResponse))
        {
            return $"<strong>{lastResponse}</strong>";
        }

        return lastResponse;
    }

}