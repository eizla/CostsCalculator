using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.Lang;
using Newtonsoft.Json;

namespace CostsCalculator.Models
{
    public class UserItem
    {
        private string id;
        private string name;
        private string description;
        private string color;

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

        [JsonProperty(PropertyName = "color")]
        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UserItem)) return false;
            return Eq((UserItem)obj);
        }

        protected bool Eq(UserItem other)
        {
            return string.Equals(id, other.id) && string.Equals(name, other.name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((id != null ? id.GetHashCode() : 0) * 397) ^ (name != null ? name.GetHashCode() : 0);
            }
        }

        public static explicit operator UserItem(Java.Lang.Object v)
        {
            throw new NotImplementedException();
        }
    }
}
