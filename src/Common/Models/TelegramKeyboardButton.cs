using System.Text.Json.Serialization;

namespace Common.Models
{
    public class TelegramKeyboardButton
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("request_location")]
        public bool RequestLocation { get; set; }
    }
}
