using Common.Helpers;
using KandilliEarthquakeBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace KandilliEarthquakeBot.Services
{
    public interface ISubscriptionUpdateRequestBuilder
    {
        SubscriptionUpdateRequest Build();
        SubscriptionUpdateRequestBuilder CreateRequest(int subscriptionId);
        SubscriptionUpdateRequestBuilder SetLocation(Location location);
        SubscriptionUpdateRequestBuilder RemoveLocation();
        SubscriptionUpdateRequestBuilder SetMagnitude(double magnitude);
    }

    public class SubscriptionUpdateRequestBuilder : ISubscriptionUpdateRequestBuilder
    {
        private SubscriptionUpdateRequest _subscriptionUpdateRequest;
        private List<UpdatedSubscriberProperty> _updatedProperties;
        private List<string> _removedProperties;

        public SubscriptionUpdateRequestBuilder CreateRequest(int subscriptionId)
        {
            _subscriptionUpdateRequest = new SubscriptionUpdateRequest
            {
                SubscriptionId = subscriptionId
            };

            _updatedProperties = new List<UpdatedSubscriberProperty>();
            _removedProperties = new List<string>();

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
            _removedProperties.Remove("Location");
            _removedProperties.Remove("LocationHash");

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

        public SubscriptionUpdateRequestBuilder RemoveLocation()
        {
            _updatedProperties = _updatedProperties.Where(p => p.Name != "Location" || p.Name != "LocationHash").ToList();
            _removedProperties.Add("Location");
            _removedProperties.Add("LocationHash");
            return this;
        }

        public SubscriptionUpdateRequest Build()
        {
            _subscriptionUpdateRequest.UpdatedProperties = _updatedProperties;
            _subscriptionUpdateRequest.RemovedProperties = _removedProperties;
            return _subscriptionUpdateRequest;
        }
    }
}
