using Newtonsoft.Json;

namespace KandilliEarthquakeBot.Models
{
    public class WebhookMessage
    {
        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("callback_query")]
        public CallbackQuery CallbackQuery { get; set; }
    }
}
