using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public class TelegramInlineKeyboardMarkup
    {
        [JsonPropertyName("inline_keyboard")]
        public IEnumerable<IEnumerable<TelegramInlineKeyboardButton>> InlineKeyboard { get; set; }
    }
}
