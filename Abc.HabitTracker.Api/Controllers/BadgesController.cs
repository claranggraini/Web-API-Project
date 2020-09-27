using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abc.HabitTracker.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Abc.HabitTracker.Api.Controllers
{
  [ApiController]
  public class BadgesController : ControllerBase
  {
    private readonly ILogger<BadgesController> _logger;

    public BadgesController(ILogger<BadgesController> logger)
    {
      _logger = logger;
    }
        [HttpGet("api/v1/users/{user_id}/badges")]
        public ActionResult<IEnumerable<Badge_Assignment>> All(Guid user_id)
        {
            var AssignedBadges = Badge_AssignmentRepository.GetAssignedBadgeByUserID(user_id);
            return Ok(AssignedBadges);
        }
    }
}
