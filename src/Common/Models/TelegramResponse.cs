using Newtonsoft.Json;

namespace Common.Models
{
    public class TelegramResponse
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; }

        [JsonProperty("description")]
        public string Description { get; }
    }
}
