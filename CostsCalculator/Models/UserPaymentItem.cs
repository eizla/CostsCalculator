using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostsCalculator.Models
{
    class UserPaymentItem
    {
        private string id;
        private string payId;
        private string userId;
        private decimal amount;

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

        [JsonProperty(PropertyName = "payId")]
        public string PayId
        {
            get { return payId; }
            set { payId = value; }
        }

        [JsonProperty(PropertyName = "amount")]
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }


    }
}
