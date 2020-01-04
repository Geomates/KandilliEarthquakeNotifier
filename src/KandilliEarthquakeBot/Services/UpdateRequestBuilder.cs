using KandilliEarthquakeBot.Helpers;
using KandilliEarthquakeBot.Models;
using System.Collections.Generic;

namespace KandilliEarthquakeBot.Services
{
    public interface ISubscriptionUpdateRequestBuilder
    {
        SubscriptionUpdateRequest Build();
        SubscriptionUpdateRequestBuilder CreateRequest(int subscriptionId);
        SubscriptionUpdateRequestBuilder SetLocation(Location location);
        SubscriptionUpdateRequestBuilder SetMagnitude(double magnitude);
    }

    public class SubscriptionUpdateRequestBuilder : ISubscriptionUpdateRequestBuilder
    {
        private SubscriptionUpdateRequest _subscriptionUpdateRequest;
        private List<UpdatedSubscriberProperty> _updatedProperties;

        public SubscriptionUpdateRequestBuilder CreateRequest(int subscriptionId)
        {
            _subscriptionUpdateRequest = new SubscriptionUpdateRequest
            {
                SubscriptionId = subscriptionId
            };

            _updatedProperties = new List<UpdatedSubscriberProperty>();

            return this;
        }

        public SubscriptionUpdateRequestBuilder SetMagnitude(double magnitude)
        {
            _updatedProperties.Add(new UpdatedSubscriberProperty
            {
                Name = "Magnitude",
                Type = typeof(double),
                Value = magnitude
            });

            return this;
        }

        public SubscriptionUpdateRequestBuilder SetLocation(Location location)
        {
            _updatedProperties.Add(new UpdatedSubscriberProperty
            {
                Name = "Location",
                Type = typeof(string),
                Value = $"{location.Latitude},{location.Longitude}"
            });

            var geohash = S2Manager.GenerateGeohash(location);

            _updatedProperties.Add(new UpdatedSubscriberProperty
            {
                Name = "LocationHash",
                Type = typeof(ulong),
                Value = geohash
            });

            _updatedProperties.Add(new UpdatedSubscriberProperty
            {
                Name = "LocationHashKey",
                Type = typeof(ulong),
                Value = S2Manager.GenerateHashKey(geohash, 4)
            });

            return this;
        }

        public SubscriptionUpdateRequest Build()
        {
            _subscriptionUpdateRequest.UpdatedProperties = _updatedProperties;
            return _subscriptionUpdateRequest;
        }
    }
}
