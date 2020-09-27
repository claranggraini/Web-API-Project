using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api.Factory
{
    public class HabitFactory
    {
        public static Habit CreateHabit(Guid user_id, string name, DaysOff daysOff, DateTime created_at)
        {
            if (name.Length < 2 || name.Length > 100) throw new Exception("Name must be between 2 and 100 characters");
            return new Habit(System.Guid.NewGuid(), user_id, name, daysOff, 0, 0, 0, created_at);
        }
    }
}
