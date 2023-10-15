using Common.Exceptions;
using Common.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface ITelegramService
    {
        Task<bool> SendMessage(TelegramMessage message);
        Task<bool> DeleteMessage(TelegramDeleteMessage telegramDeleteMessage);
        Task<bool> AnswerCallbackQuery(AnswerCallbackQuery answerCallbackQuery);
    }

    public class TelegramService : ITelegramService
    {
        private const string TelegramApiUrl = "https://api.telegram.org";
        private const string TelegramApiToken = "TELEGRAM_API_TOKEN";

        private readonly string _apiToken;

        public TelegramService(IEnvironmentService environmentService)
        {
            _apiToken = environmentService.GetEnvironmentValue(TelegramApiToken);
        }

        public async Task<bool> DeleteMessage(TelegramDeleteMessage telegramDeleteMessage)
        {
            var url = $"{TelegramApiUrl}/bot{_apiToken}/deleteMessage";
            var content = new StringContent(JsonSerializer.Serialize(telegramDeleteMessage, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }), Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(url, content)) 
                return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendMessage(TelegramMessage message)
        {
            var url = $"{TelegramApiUrl}/bot{_apiToken}/sendMessage";
            var content = new StringContent(JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }), Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(url, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new TelegramApiException(await response.Content.ReadAsStringAsync());
                }
                return true;
            }
        }

        public async Task<bool> AnswerCallbackQuery(AnswerCallbackQuery answerCallbackQuery)
        {
            var url = $"{TelegramApiUrl}/bot{_apiToken}/answerCallbackQuery";
            var content = new StringContent(JsonSerializer.Serialize(answerCallbackQuery, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }), Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(url, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
                return true;
            }
        }
    }
}
