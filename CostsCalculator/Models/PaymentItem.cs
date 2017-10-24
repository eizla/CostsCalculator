using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostsCalculator.Models
{
    public class PaymentItem
    {
        private string id;
        private string name;
        private string tripId;
        private decimal amount;
        private string userCreatingPaymentId;


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

        [JsonProperty(PropertyName = "tripId")]
        public string TripId
        {
            get { return tripId; }
            set { tripId = value; }
        }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }


        [JsonProperty(PropertyName = "userCreatingPaymentId")]
        public string UserCreatingPaymentId
        {
            get { return userCreatingPaymentId; }
            set { userCreatingPaymentId = value; }
        }

    }
}
