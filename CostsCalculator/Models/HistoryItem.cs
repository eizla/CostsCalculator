using System;
using Newtonsoft.Json;

namespace CostsCalculator.Models
{
    public class HistoryItem
    {
        private string id;
        private string tripId;
        private string userId;
        private string description;
        private DateTime date;

        public HistoryItem(string tripId, string userId, string description)
        {
            this.tripId = tripId;
            this.userId = userId;
            this.description = description;
            this.date = DateTime.Now;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "userId")]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [JsonProperty(PropertyName = "tripId")]
        public string TripId
        {
            get { return tripId; }
            set { tripId = value; }
        }
        
        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public override string ToString()
        {
            return description +  " " + date;
        }
    }
}
