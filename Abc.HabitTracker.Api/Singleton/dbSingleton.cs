using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api.Singleton
{
    public class dbSingleton
    {
        private dbSingleton()
        {

        }
        private static HabitTrackerDataSet db = null;
        public static HabitTrackerDataSet getInstance()
        {
            if (db == null)
            {
                db = new HabitTrackerDataSet();
            }
            return db;
        }
    }
}
