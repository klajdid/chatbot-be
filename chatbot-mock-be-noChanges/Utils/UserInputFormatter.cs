using System.Text.RegularExpressions;

namespace chatbot_mock_be.Utils;

public class UserInputFormatter
{ 
    public static string RemoveFormatting(string text)
    {
        string pattern = @"<[^>]+>|&nbsp;";
        string cleanedText = Regex.Replace(text, pattern, "");
        return cleanedText;
    }
}