using Common.Helpers;
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

            _updatedProperties.Add(new UpdatedSubscriberProperty
            {
                Name = "LocationHash",
                Type = typeof(ulong),
                Value = S2Manager.GenerateGeohash(location.Latitude, location.Longitude)
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
