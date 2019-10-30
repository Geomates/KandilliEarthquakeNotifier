using KandilliEarthquakeNotifier.Entities;
using System.Text;

namespace KandilliEarthquakeNotifier
{
    public static class Extensions
    {
        public static string ToTelegramMessage(this Earthquake earthquake)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Yer: {earthquake.Location}");
            stringBuilder.AppendLine($"Buyukluk: {earthquake.Magnitude}");
            stringBuilder.AppendLine($"Zaman: {earthquake.Date.ToString("dd/MM/yyyy HH:mm")}");

            return stringBuilder.ToString();
        }
    }
}
