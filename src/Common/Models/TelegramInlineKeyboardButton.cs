using Newtonsoft.Json;

namespace Common.Models
{
    public class TelegramInlineKeyboardButton
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        
        [JsonProperty("callback_data")]
        public string CallBackData { get; set; }
    }
}
