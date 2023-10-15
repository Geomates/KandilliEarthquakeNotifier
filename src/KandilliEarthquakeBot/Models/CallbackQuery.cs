using System.Text.Json.Serialization;

namespace KandilliEarthquakeBot.Models
{
    public class CallbackQuery
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
