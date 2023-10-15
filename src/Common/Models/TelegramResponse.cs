using System.Text.Json.Serialization;

namespace Common.Models
{
    public class TelegramResponse
    {
        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
