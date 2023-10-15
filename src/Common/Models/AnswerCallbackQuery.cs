using System.Text.Json.Serialization;

namespace Common.Models
{
    public class AnswerCallbackQuery
    {
        [JsonPropertyName("callback_query_id")]
        public string CallbackQueryId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("show_alert")]
        public bool ShowAlert { get; set; }
    }
}
