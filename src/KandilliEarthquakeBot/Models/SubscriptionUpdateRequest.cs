using System.Collections.Generic;

namespace KandilliEarthquakeBot.Models
{
    public class SubscriptionUpdateRequest
    {
        public int SubscriptionId { get; set; }
        public IEnumerable<UpdatedSubscriberProperty> UpdatedProperties { get; set; }
        public IEnumerable<string> RemovedProperties { get; set; }
    }
}
