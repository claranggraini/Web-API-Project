using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api.Factory
{
    public class Badge_AssignmentFactory
    {
        public static Badge_Assignment CreateBadgeAssignment(Guid user_id, Badge badge, DateTime created_at)
        {
            return new Badge_Assignment(user_id, badge, created_at); 
        }
    }
}
