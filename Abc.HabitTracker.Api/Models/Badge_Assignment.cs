using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api
{
    public class Badge_Assignment
    {
        [JsonPropertyName("user_id")]
        public Guid UserID { get; set; }

        [JsonPropertyName("badge")]
        public Badge _Badge { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        public Badge_Assignment(Guid user_id, Badge badge, DateTime created_at)
        {
            if (user_id.Equals(null)) throw new Exception("UserID is required");
            if (created_at.Equals(null)) throw new Exception("Created_At is required");
            UserID = user_id;
            _Badge = badge;
            CreatedAt = created_at;
        }
    }
}
