using Newtonsoft.Json;
using System.Collections.Generic;

namespace Common.Models
{
    public class TelegramReplyKeyboardMarkup
    {
        [JsonProperty("keyboard")]
        public IEnumerable<IEnumerable<TelegramKeyboardButton>> Keyboard { get; set; }

        [JsonProperty("one_time_keyboard")]
        public bool OneTimeKeyboard { get; set; }
    }
}
