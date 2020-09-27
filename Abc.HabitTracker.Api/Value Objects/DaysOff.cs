using Abc.HabitTracker.Api.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api
{
    public class DaysOff : ValueObject<DaysOff>
    {
        [JsonPropertyName("value")]
        public string[] value { get; }

        public DaysOff(string [] value)
        {
            if (value.Length >= 7) throw new Exception("Days count must not equal to or more than 7 days");
            if (!validateDays(value)) throw new Exception("Days [Mon | Tue | Wed | Thu | Fri | Sat | Sun] and must not duplicate");

            this.value = value;
        }

        private bool validateDays(string [] value)
        {
            for(int i = 0; i < value.Length; i++)
            {
                if (!value[i].Equals("Mon") && !value[i].Equals("Tue") && !value[i].Equals("Wed") && 
                    !value[i].Equals("Thu") && !value[i].Equals("Fri") && !value[i].Equals("Sat") && 
                    !value[i].Equals("Sun")) return false;
                for (int j = i+1; j < value.Length; j++)
                {
                    if (value[j].Equals(value[i])) return false;
                }
            }
            return true;
        }

        protected override bool EqualsCore(DaysOff other)
        {
            return value.Equals(other.value);
        }

        protected override int GetHashCodeCore()
        {
            unchecked
            {
                int hashCode = value.GetHashCode();

                return hashCode;
            }
        }
    }
}
