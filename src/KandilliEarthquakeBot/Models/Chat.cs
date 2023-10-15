using System.Text.Json.Serialization;

namespace KandilliEarthquakeBot.Models
{
    public class Chat
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
