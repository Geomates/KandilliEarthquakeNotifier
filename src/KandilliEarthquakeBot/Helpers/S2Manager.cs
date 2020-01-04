using Google.Common.Geometry;
using KandilliEarthquakeBot.Models;
using System;
using System.Globalization;

namespace KandilliEarthquakeBot.Helpers
{
    public static class S2Manager
    {
        public static ulong GenerateGeohash(Location location)
        {
            var latLng = S2LatLng.FromDegrees(location.Latitude, location.Longitude);
            var cell = new S2Cell(latLng);
            var cellId = cell.Id;

            return cellId.Id;
        }

        public static ulong GenerateHashKey(ulong geohash, int hashKeyLength)
        {
            var geohashString = geohash.ToString(CultureInfo.InvariantCulture);
            var denominator = (ulong)Math.Pow(10, geohashString.Length - hashKeyLength);
            return geohash / denominator;
        }
    }
}
