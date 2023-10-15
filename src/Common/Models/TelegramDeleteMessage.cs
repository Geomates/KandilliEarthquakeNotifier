using System.Text.Json.Serialization;

namespace Common.Models
{
    public class TelegramDeleteMessage
    {
        [JsonPropertyName("chat_id")]
        public int ChatId { get; set; }

        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }
    }
}
