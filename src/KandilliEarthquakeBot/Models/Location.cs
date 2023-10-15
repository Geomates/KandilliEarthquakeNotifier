using System.Text.Json.Serialization;

namespace KandilliEarthquakeBot.Models
{
    public class Location
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
