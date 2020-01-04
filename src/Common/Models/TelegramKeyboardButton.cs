using Newtonsoft.Json;

namespace Common.Models
{
    public class TelegramKeyboardButton
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("request_location")]
        public bool RequestLocation { get; set; }
    }
}
