using Abc.HabitTracker.Api.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api.Value_Objects
{
    public class Log
    {
        [JsonPropertyName("id")]
        public Guid Habit_ID { get; set; }

        [JsonPropertyName("logs")]
        public DateTime Date_Log { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserID { get; set; }

        //private readonly HabitRepository HabitRepo;
        public Log(Guid habit_id, Guid user_id, DateTime date_log)
        {
            Habit_ID = habit_id;
            UserID = user_id;
            Date_Log = date_log;
        }

    }
}
