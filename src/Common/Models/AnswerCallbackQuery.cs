using Newtonsoft.Json;

namespace Common.Models
{
    public class AnswerCallbackQuery
    {
        [JsonProperty("callback_query_id")]
        public string CallbackQueryId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("show_alert")]
        public bool ShowAlert { get; set; }
    }
}
