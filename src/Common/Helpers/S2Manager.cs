using Google.Common.Geometry;

namespace Common.Helpers
{
    public static class S2Manager
    {
        public static S2LatLngRect GetBoundingLatLngRect(double latitude, double longitude, int radius)
        {
            var centerLatLng = S2LatLng.FromDegrees(latitude, longitude);

            var latReferenceUnit = latitude > 0.0 ? -1.0 : 1.0;
            var latReferenceLatLng = S2LatLng.FromDegrees(latitude + latReferenceUnit, longitude);
            var lngReferenceUnit = longitude > 0.0 ? -1.0 : 1.0;
            var lngReferenceLatLng = S2LatLng.FromDegrees(latitude, longitude + lngReferenceUnit);

            var latForRadius = radius / centerLatLng.GetEarthDistance(latReferenceLatLng);
            var lngForRadius = radius / centerLatLng.GetEarthDistance(lngReferenceLatLng);

            var minLatLng = S2LatLng.FromDegrees(latitude - latForRadius, longitude - lngForRadius);
            var maxLatLng = S2LatLng.FromDegrees(latitude + latForRadius, longitude + lngForRadius);

            return new S2LatLngRect(minLatLng, maxLatLng);
        }

        public static ulong GenerateGeohash(double latitude, double longitude)
        {
            var latLng = S2LatLng.FromDegrees(latitude, longitude);
            var cell = new S2Cell(latLng);
            var cellId = cell.Id;

            return cellId.Id;
        }
    }
}
