using System.Net.Http;
using System.Threading.Tasks;

namespace KandilliEarthquakeNotifier.Services
{
    public interface ITelegramService
    {
        Task<bool> SendMessage(string message, bool isSilent);
    }

    public class TelegramService : ITelegramService
    {
        private const string TELEGRAM_API_URL = "https://api.telegram.org";
        private const string TELEGRAM_API_TOKEN = "TELEGRAM_API_TOKEN";
        private const string TELEGRAM_CHANNEL_NAME = "TELEGRAM_CHANNEL_NAME";

        private readonly string _apiToken;
        private readonly string _channelName;

        public TelegramService(IEnvironmentService environmentService)
        {
            _apiToken = environmentService.GetEnvironmentValue(TELEGRAM_API_TOKEN);
            _channelName = environmentService.GetEnvironmentValue(TELEGRAM_CHANNEL_NAME);
        }


        public async Task<bool> SendMessage(string message, bool isSilent = false)
        {
            string urlString = $"{TELEGRAM_API_URL}/bot{_apiToken}/sendMessage?chat_id=@{_channelName}&text={message}&disable_notification={isSilent}";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage reponse = await client.GetAsync(urlString))
            {
                return reponse.IsSuccessStatusCode;
            }
        }
    }
}
