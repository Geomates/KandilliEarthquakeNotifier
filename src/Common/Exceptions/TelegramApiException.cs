using Common.Models;
using Newtonsoft.Json;
using System;

namespace Common.Exceptions
{
    public class TelegramApiException : Exception
    {
        public TelegramResponse Response { get; }
        public TelegramApiException(TelegramResponse response)
            :base(JsonConvert.SerializeObject(response))
        {
            Response = response;
        }
    }
}
