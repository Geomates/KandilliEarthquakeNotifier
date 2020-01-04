using Newtonsoft.Json;

namespace Common.Models
{
    public class TelegramDeleteMessage
    {
        [JsonProperty("chat_id")]
        public int ChatId { get; set; }

        [JsonProperty("message_id")]
        public int MessageId { get; set; }
    }
}
