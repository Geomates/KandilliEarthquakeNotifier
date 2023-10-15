using System.Text.Json.Serialization;

namespace KandilliEarthquakeBot.Models
{
    public class Message
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }

        [JsonPropertyName("chat")]
        public Chat Chat { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }
    }
}
