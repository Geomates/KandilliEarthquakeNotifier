using KandilliEarthquakePuller.Models;
using System.Text;

namespace KandilliEarthquakePuller
{
    public static class Extensions
    {
        private const string GOOGLE_MAPS_SEARCH_API_URL = "https://www.google.com/maps/search/";
        public static string ToTelegramMessage(this Earthquake earthquake)
        {
            var stringBuilder = new StringBuilder();

            var googleMapsUrl = $"{GOOGLE_MAPS_SEARCH_API_URL}?api=1&query={earthquake.Latitude},{earthquake.Longitude}";

            stringBuilder.AppendLine($"Yer: [{earthquake.Location}]({googleMapsUrl})");
            stringBuilder.AppendLine($"Buyukluk: {earthquake.Magnitude}");
            stringBuilder.AppendLine($"Zaman: {earthquake.Date.ToString("dd/MM/yyyy HH:mm")}");

            return stringBuilder.ToString();
        }
    }
}
