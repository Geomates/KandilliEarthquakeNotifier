using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public class TelegramReplyKeyboardMarkup
    {
        [JsonPropertyName("keyboard")]
        public IEnumerable<IEnumerable<TelegramKeyboardButton>> Keyboard { get; set; }

        [JsonPropertyName("one_time_keyboard")]
        public bool OneTimeKeyboard { get; set; }
    }
}
