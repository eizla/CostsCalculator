using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CostsCalculator.Models
{
    public class TripItem
    {
        private string id;
        private string name;
        private string description;
        private DateTime startDate;
        private DateTime endDate;
        private string ownerId;
        private bool isCurrent;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        [JsonProperty(PropertyName = "endDate")]
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        [JsonProperty(PropertyName = "ownerId")]
        public string OwnerId
        {
            get { return ownerId; }
            set { ownerId = value; }
        }

        public override String ToString()
        {
            return Name + "\nDescription: " + Description + "\nDate:\nFrom: " + StartDate.ToString("d MMM yyyy") +
                   " to: " + EndDate.ToString("d MMM yyyy");
        }

        [JsonProperty(PropertyName = "isCurrent")]
        public bool IsCurrent
        {
            get { return isCurrent; }
            set { isCurrent = value; }
        }

    }

}
