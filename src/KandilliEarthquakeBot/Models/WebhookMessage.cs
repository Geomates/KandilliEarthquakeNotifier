using System.Text.Json.Serialization;

namespace KandilliEarthquakeBot.Models
{
    public class WebhookMessage
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("callback_query")]
        public CallbackQuery CallbackQuery { get; set; }
    }
}
