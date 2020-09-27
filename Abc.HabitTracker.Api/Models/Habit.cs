using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Abc.HabitTracker.Api.Value_Objects;
using System.Linq;
using Abc.HabitTracker.Api.DomainEvents;
using Abc.HabitTracker.Api.Repository;

namespace Abc.HabitTracker.Api
{
  public class RequestData
  {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("days_off")]
        public string[] DaysOff { get; set; }
    }

    public class Habit : AggregateRoot, IDomainService
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("days_off")]
        public DaysOff DaysOff { get; set; }

        [JsonPropertyName("current_streak")]
        public int CurrentStreak { get; set; }

        [JsonPropertyName("longest_streak")]
        public int LongestStreak { get; set; }

        [JsonPropertyName("log_count")]
        public int LogCount { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserID { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        
        public static List<IDomainEvent> events = new List<IDomainEvent>();

        public Habit(Guid id, Guid user_id, string name, DaysOff daysOff, int current_streak, int longest_streak, int log_count, DateTime created_at)
        {
            ID = id;
            UserID = user_id;
            Name = name;
            DaysOff = daysOff;
            CurrentStreak = current_streak;
            LongestStreak = longest_streak;
            LogCount = log_count;
            CreatedAt = created_at;
        }

        public void LogIn(DateTime Date_Log)
        {
            if (LogCount > 0)
            {
                IEnumerable<Log> logs = HabitRepository.GetLogDataByID(ID, UserID);
                if (logs.Count() > 0 && (Date_Log.Date - logs.ElementAt(logs.Count() - 1).Date_Log.Date).Days == 1)
                {
                    CurrentStreak++;
                }
                else if (!Date_Log.Date.Equals(logs.ElementAt(logs.Count() - 1).Date_Log.Date))
                {
                    CurrentStreak = 1;
                }
            }
            else
            {
                CurrentStreak = 1;
            }

            if (LongestStreak < CurrentStreak)
            {
                LongestStreak = CurrentStreak;
                HabitRepository.UpdateLongestStreak(ID, LongestStreak);
            }

            HabitRepository.UpdateCurrentStreak(ID, CurrentStreak);
            HabitRepository.UpdateLogCount(ID, ++LogCount);
            Log log = new Log(ID, UserID, Date_Log);
            HabitRepository.SaveLogData(log);
            events.Add(new HabitLoggedInEvent(this, log));
        }
    }
}
