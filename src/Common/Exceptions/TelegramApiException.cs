using Common.Models;
using System;
using System.Text.Json;

namespace Common.Exceptions
{
    public class TelegramApiException : Exception
    {
        public TelegramResponse Response { get; }

        public TelegramApiException(string response)
            :base(response)
        {
            Response = JsonSerializer.Deserialize<TelegramResponse>(response);
        }
    }
}
