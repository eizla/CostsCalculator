using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CostsCalculator.Models
{
    class FriendItem
    {
        private string id;
        private string userId;
        private string friendId;

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

        [JsonProperty(PropertyName = "friendId")]
        public string FriendId
        {
            get { return friendId; }
            set { friendId = value; }
        }
    }
}
