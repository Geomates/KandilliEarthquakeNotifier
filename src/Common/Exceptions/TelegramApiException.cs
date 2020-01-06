using Common.Models;
using Newtonsoft.Json;
using System;

namespace Common.Exceptions
{
    public class TelegramApiException : Exception
    {
        public TelegramResponse Response { get; }

        public TelegramApiException(string response)
            :base(response)
        {
            Response = JsonConvert.DeserializeObject<TelegramResponse>(response);
        }
    }
}
