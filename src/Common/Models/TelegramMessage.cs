using System.Text.Json.Serialization;

namespace Common.Models
{
    public class TelegramMessage
    {
        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("parse_mode")]
        public string ParseMode { get; set; }

        [JsonPropertyName("disable_web_page_preview")]
        public bool DisableWebPagePreview { get; set; }

        [JsonPropertyName("disable_notification")]
        public bool DisableNotification { get; set; }

        [JsonPropertyName("reply_markup")]
        public string ReplyMarkup { get; set; }
    }
}
