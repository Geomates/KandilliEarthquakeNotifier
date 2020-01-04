using Newtonsoft.Json;
using System.Collections.Generic;

namespace Common.Models
{
    public class TelegramInlineKeyboardMarkup
    {
        [JsonProperty("inline_keyboard")]
        public IEnumerable<IEnumerable<TelegramInlineKeyboardButton>> InlineKeyboard { get; set; }
    }
}
