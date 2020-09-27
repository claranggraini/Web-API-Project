using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api
{
    public sealed class Badge : ValueObject<Badge>
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }


        public Badge(string name, string description)
        {
            if (!name.Equals("Workaholic") && !name.Equals("Dominating") && !name.Equals("Epic Comeback")) throw new Exception("Badge [Dominating | Workaholic | Epic Comeback ]");
            Name = name;
            Description = description;
        }

        protected override bool EqualsCore(Badge other)
        {
            return Name.Equals(other.Name);
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                return Name.GetHashCode();
            }
        }
    }
}
