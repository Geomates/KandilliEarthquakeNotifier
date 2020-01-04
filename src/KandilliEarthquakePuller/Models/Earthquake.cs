using System;

namespace KandilliEarthquakePuller.Models
{
    public class Earthquake
    {
        public DateTime Date { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Depth { get; set; }
        public double Magnitude { get; set; }
        public string Location { get; set; }
    }
}