using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abc.HabitTracker.Api.DomainEvents;
using Abc.HabitTracker.Api.Repository;
using Abc.HabitTracker.Api.Singleton;
using Abc.HabitTracker.Api.Value_Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;

namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class HabitsController : ControllerBase
    {
        private readonly ILogger<HabitsController> _logger;

        public HabitsController(ILogger<HabitsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("api/v1/users/{user_id}/habits")]
        public ActionResult<IEnumerable<Habit>> All(Guid user_id)
        {
            var UserHabits = HabitRepository.GetHabitByUserId(user_id);
            if (UserHabits != null)
            {
                return Ok(UserHabits);
            }
            return NotFound("user not found");
        }

        [HttpGet("api/v1/users/{user_id}/habits/{id}")]
        public ActionResult<Habit> Get(Guid user_id, Guid id)
        {
            Habit h = HabitRepository.GetHabitByID(id, user_id);
            if (h != null)
            {
                return Ok(h);
            }
           return NotFound("habit not found");
        }

        [HttpPost("api/v1/users/{user_id}/habits")]
        public ActionResult<Habit> AddNewHabit(Guid user_id, [FromBody] RequestData data)
        {
            DaysOff daysoff = new DaysOff(data.DaysOff);
            HabitRepository.RegisterHabit(user_id, data.Name, daysoff, DateTime.Now);
            return Ok(HabitRepository.GetNewestHabit(user_id));
        }

        [HttpPut("api/v1/users/{user_id}/habits/{id}")]
        public ActionResult<Habit> UpdateHabit(Guid user_id, Guid id, [FromBody] RequestData data)
        {
            Habit h = HabitRepository.GetHabitByID(id, user_id);
            if (h != null)
            {
                DaysOff daysoff = new DaysOff(data.DaysOff);
                HabitRepository.UpdateHabitData(id, user_id, data.Name, daysoff);
                return Ok(h);
            }
            return NotFound("habit not found");
        }

        [HttpDelete("api/v1/users/{user_id}/habits/{id}")]
        public ActionResult<Habit> DeleteHabit(Guid user_id, Guid id)
        {

            Habit h = HabitRepository.GetHabitByID(id, user_id);
            if (h != null)
            {
                HabitRepository.DeleteHabitData(id, user_id);
                return Ok(h);
            }

            return NotFound("Habit not found");
        }

        [HttpPost("api/v1/users/{user_id}/habits/{id}/logs")]
        public ActionResult<Habit> Log(Guid user_id, Guid id)
        {
            Habit h = HabitRepository.GetHabitByID(id, user_id);
            h.LogIn(DateTime.Now);

            return Ok(h);
        }
    }
}
