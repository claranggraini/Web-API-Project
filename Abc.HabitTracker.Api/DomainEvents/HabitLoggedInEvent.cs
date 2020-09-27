using Abc.HabitTracker.Api.Repository;
using Abc.HabitTracker.Api.Value_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api.DomainEvents
{
    public class HabitLoggedInEvent : IDomainEvent
    {
        private Habit _habit { get; set; }
        private Log _log { get; set; }
        public HabitLoggedInEvent(Habit habit, Log log)
        {
            _habit = habit;
            _log = log;
            ValidateBadgeAssignment();
        }

       
        public void ValidateBadgeAssignment()
        {
            IEnumerable <Log> logs = HabitRepository.GetLogDataByID(_habit.ID, _habit.UserID);
            IEnumerable<Badge_Assignment> AssignedBadges = Badge_AssignmentRepository.GetAssignedBadgeByUserID(_habit.UserID);
            List<string> badges = new List<string>();

            if (AssignedBadges.Count() < 3)
            {
                foreach (Badge_Assignment bd in AssignedBadges)
                {
                    badges.Add(bd._Badge.Name);
                }
                if (!badges.Contains("Dominating") && CheckDominating())
                {
                    Badge_AssignmentRepository.SaveBadgeAssignmentData(_habit.UserID, "Dominating", _log.Date_Log);
                }if(!badges.Contains("Workaholic") && CheckWorkaholic())
                {
                    Badge_AssignmentRepository.SaveBadgeAssignmentData(_habit.UserID, "Workaholic", _log.Date_Log);
                }
                if (!badges.Contains("Epic Comeback") && CheckEpic(_habit, logs))
                {
                    Badge_AssignmentRepository.SaveBadgeAssignmentData(_habit.UserID, "Epic Comeback", _log.Date_Log);
                }
            }

        }

        public bool CheckDominating()
        {
            if (_habit.CurrentStreak == 4) return true;
            
            return false;
        }

        public bool CheckWorkaholic()
        {
            int streak = 0;
            IEnumerable<Habit> hList = HabitRepository.GetHabitByUserId(_habit.UserID);
            foreach (Habit hb in hList)
            {
                IEnumerable<Log> logList = HabitRepository.GetLogDataByID(hb.ID, hb.UserID);
                Log temp = null;
                foreach (Log l in logList)
                {
                    if (streak == 0)
                    {
                       temp = l;
                    }
                    else if((l.Date_Log.Date-temp.Date_Log.Date).Days != 0)
                    {
                        continue;   
                    }
                    foreach (string s in hb.DaysOff.value)
                    {
                        if (l.Date_Log.DayOfWeek.ToString().StartsWith(s))
                        {
                            ++streak;
                            if (streak >= 10) return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CheckEpic(Habit _habit, IEnumerable<Log> logs)
        {
            if (_habit.CurrentStreak == 10)
            {
                Log temp = logs.ElementAt(logs.Count()-1);
                for (int i = logs.Count()-2; i >= 0; i--)
                {
                    if ((temp.Date_Log.Date - logs.ElementAt(i).Date_Log.Date).Days >= 9)
                    {
                        return true;
                    }
                    temp = logs.ElementAt(i);
                }
            }
            return false;
        }

    }
}
