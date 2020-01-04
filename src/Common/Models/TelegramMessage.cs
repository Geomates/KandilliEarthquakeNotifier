using Newtonsoft.Json;

namespace Common.Models
{
    public class TelegramMessage
    {
        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("parse_mode")]
        public string ParseMode { get; set; }

        [JsonProperty("disable_web_page_preview")]
        public bool DisableWebPagePreview { get; set; }

        [JsonProperty("disable_notification")]
        public bool DisableNotification { get; set; }

        [JsonProperty("reply_markup")]
        public string ReplyMarkup { get; set; }
    }
}
