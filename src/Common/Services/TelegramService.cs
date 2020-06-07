using Common.Exceptions;
using Common.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
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
        private const string TELEGRAM_API_URL = "https://api.telegram.org";
        private const string TELEGRAM_API_TOKEN = "TELEGRAM_API_TOKEN";

        private readonly string _apiToken;

        public TelegramService(IEnvironmentService environmentService)
        {
            _apiToken = environmentService.GetEnvironmentValue(TELEGRAM_API_TOKEN);
        }

        public async Task<bool> DeleteMessage(TelegramDeleteMessage telegramDeleteMessage)
        {
            var url = $"{TELEGRAM_API_URL}/bot{_apiToken}/deleteMessage";
            var content = new StringContent(JsonConvert.SerializeObject(telegramDeleteMessage, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }), Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(url, content)) 
                return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendMessage(TelegramMessage message)
        {
            var url = $"{TELEGRAM_API_URL}/bot{_apiToken}/sendMessage";
            var content = new StringContent(JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
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
            var url = $"{TELEGRAM_API_URL}/bot{_apiToken}/answerCallbackQuery";
            var content = new StringContent(JsonConvert.SerializeObject(answerCallbackQuery, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
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
