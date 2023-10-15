using System.Text.Json.Serialization;

namespace Common.Models
{
    public class TelegramInlineKeyboardButton
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        
        [JsonPropertyName("callback_data")]
        public string CallBackData { get; set; }
    }
}
