using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api
{
    interface IDomainService
    {
        public abstract void LogIn(DateTime Date_Log);
    }
}
