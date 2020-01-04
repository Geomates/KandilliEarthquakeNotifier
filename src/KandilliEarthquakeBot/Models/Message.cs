namespace KandilliEarthquakeBot.Models
{
    public class Message
    {
        public Chat Chat { get; set; }
        public string Text { get; set; }
        public Location Location { get; set; }
    }
}
