using Abc.HabitTracker.Api.Factory;
using Abc.HabitTracker.Api.Singleton;
using Abc.HabitTracker.Api.Value_Objects;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api.Repository
{
    public class HabitRepository
    {
        private static HabitTrackerDataSet db = dbSingleton.getInstance();
        private static SqlConnection myConnection = new SqlConnection("Data Source=.;Initial Catalog=HabitTracker;Integrated Security=True");
        private static SqlDataAdapter DataAdapter(string TableName) {
            SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter("Select * from " + TableName, myConnection);
            SqlCommandBuilder mySqlCommandBuilder = new SqlCommandBuilder(mySqlDataAdapter);
            mySqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            mySqlDataAdapter.Fill(db, TableName);
            return mySqlDataAdapter;
        }
        public static void RegisterHabit(Guid user_id, string name, DaysOff daysOff, DateTime created_at)
        {
            Habit h = HabitFactory.CreateHabit(user_id, name, daysOff, created_at);

            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");

            db.Habit.AddHabitRow(h.ID, h.UserID, h.Name, h.CurrentStreak, h.LongestStreak, h.LogCount, h.CreatedAt);

            mySqlDataAdapter.Update(db, "Habit");

            if (daysOff !=null)
            {
                AssignDaysOff(h.ID, h.DaysOff);
            }
        }

        public static void AssignDaysOff(Guid habit_id, DaysOff daysOff)
        {
            List<int> DaysIDList = new List<int>();
            DaysIDList = GetDaysID(daysOff);

            SqlDataAdapter mySqlDataAdapter = DataAdapter("Days_Off");

            foreach (int i in DaysIDList)
            {
                DataRow dr = db.Days_Off.NewRow();
                dr["Habit_Id"] = habit_id.ToString();
                dr["Days_Id"] = i.ToString();
                db.Tables["Days_Off"].Rows.Add(dr);
                mySqlDataAdapter.Update(db, "Days_Off");
            }

        }

        public static IEnumerable<Habit> GetHabitByUserId (Guid user_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");

            var UserHabits = new List<Habit>();

            foreach (DataRow HRow in db.Habit)
            {
                if (HRow["User_Id"].Equals(user_id))
                {
                    UserHabits.Add(new Habit((Guid)HRow[0], user_id, HRow[2].ToString(), GetDaysOff(HRow), (int)HRow[3], (int)HRow[4], (int)HRow[5], (DateTime)HRow[6]));
                }
            }

            return UserHabits;
        }

        private static List<int> GetDaysID(DaysOff days)
        {

            SqlDataAdapter mySqlDataAdapter = DataAdapter("Days");

            List<int> daysIDList = new List<int>();
            for (int i = 0; i < days.value.Length; i++)
            {
                for (int row = 0; row < db.Days.Rows.Count; row++)
                {
                    if (db.Days.Rows[row]["Name"].Equals(days.value.ElementAt(i)))
                    {
                        daysIDList.Add(int.Parse(db.Days.Rows[row]["Id"].ToString()));
                    }
                }
            }
            return daysIDList;
        }

        private static DaysOff GetDaysOff(DataRow HRow)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Days_Off");
            int index = 0;
            List<string> arrList = new List<string>();
            foreach (DataRow row in db.Days_Off.Rows)
            {
                if (row["Habit_Id"].Equals(HRow["Id"]))
                {
                    string days = GetDaysByID((int)row["Days_Id"]);
                    if (days != null)
                    {
                        arrList.Add(days);
                    }
                }
            }
            string[] value = new string[arrList.Count];
            foreach (string i in arrList)
            {
                value[index++] = i;
            }

            DaysOff daysOff = new DaysOff(value);
            return daysOff;
        }

        private static string GetDaysByID(int Days_Id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Days");
            DataRow row = db.Days.Rows.Find(Days_Id);
            if (row != null) return row["Name"].ToString();
            return null;
        }

        public static void UpdateHabitData(Guid id, Guid user_id, string name, DaysOff daysOff)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            DataRow row = db.Habit.Rows.Find(id);

            if (row!=null && row["User_Id"].Equals(user_id))
            {
                row.BeginEdit();
                row["Name"] = name;
                UpdateDaysOff(id, user_id, daysOff);
                row.EndEdit();
                mySqlDataAdapter.Update(db, "Habit");
            }
        }

        public static Habit GetHabitByID(Guid id, Guid user_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            DataRow row = db.Habit.Rows.Find(id);
            if (row!=null && row["User_Id"].Equals(user_id))
            {
                return new Habit((Guid)row[0], (Guid)row[1], row[2].ToString(), GetDaysOff(row), (int)row[3], (int)row[4], (int)row[5], (DateTime)row[6]);
            }
            return null;
        }

        public static Habit GetNewestHabit(Guid user_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            DataRow row = db.Habit.ElementAt(db.Habit.Count-1);
            if (row != null && row["User_Id"].Equals(user_id))
            {
                return new Habit((Guid)row[0], (Guid)row[1], row[2].ToString(), GetDaysOff(row), (int)row[3], (int)row[4], (int)row[5], (DateTime)row[6]);
            }
            return null;
        }

        public static Guid GetUserId(Guid habit_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            DataRow row = db.Habit.Rows.Find(habit_id);
            if (row != null) return (Guid)row["User_Id"];
            return Guid.Empty;
        }
        private static void UpdateDaysOff(Guid habit_id, Guid user_id, DaysOff daysOff)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Days_Off");
            int index = 0;
           
            foreach (DataRow row in db.Days_Off.Rows)
            {
                if (row["Habit_Id"].Equals(habit_id))
                {
                    List<int> daysId = GetDaysID(daysOff);
                    row.BeginEdit();
                    row["Days_Id"] = daysId.ElementAt(index++).ToString();
                    row.EndEdit();
                }
            }
            mySqlDataAdapter.Update(db, "Days_Off");
        }

        private static void DeleteDaysOff(Guid habit_id, Guid user_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Days_Off");
            
            foreach (DataRow row in db.Days_Off.Rows)
            {
                if (row["Habit_Id"].Equals(habit_id))
                {
                        row.Delete();
                }
            }
            mySqlDataAdapter.Update(db, "Days_Off");
            
        }

        public static void DeleteHabitData(Guid id, Guid user_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            DataRow row = db.Habit.Rows.Find(id);
            if (row!=null && row["User_Id"].Equals(user_id))
            {
                DeleteDaysOff(id,user_id);
                row.Delete();
                mySqlDataAdapter.Update(db, "Habit");
            }
        }

        public static void SaveLogData(Log log)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Log");
            if (GetHabitByID(log.Habit_ID, log.UserID) != null)
            {
                DataRow row = db.Log.NewRow();
                row["Habit_Id"] = log.Habit_ID;
                row["Date_Log"] = log.Date_Log;
                db.Log.Rows.Add(row);
                mySqlDataAdapter.Update(db, "Log");
            }
        }

        public static IEnumerable<Log> GetLogDataByID(Guid habit_id, Guid user_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Log");
            var l = new List<Log>();
            if (GetHabitByID(habit_id, user_id) != null)
            {
                foreach (DataRow row in db.Log.Rows)
                {
                    if (row["Habit_Id"].Equals(habit_id))
                    {
                        l.Add(new Log((Guid)row["Habit_Id"], user_id, (DateTime)row["Date_Log"]));
                    }
                }
            }
            return l;
        }

        public static void UpdateLogCount(Guid habit_id,int log_count)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            if(db.Habit.Rows.Find(habit_id) != null)
            {
                DataRow row = db.Habit.Rows.Find(habit_id);
                    row.BeginEdit();
                    row["Log_Count"] = log_count;
                    row.EndEdit();
                mySqlDataAdapter.Update(db, "Habit");
            }
        }

        public static void UpdateCurrentStreak(Guid habit_id, int current_streak)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            if (GetHabitByID(habit_id, GetUserId(habit_id)) != null)
            {
                DataRow row = db.Habit.Rows.Find(habit_id);
                row.BeginEdit();
                row["Current_Streak"] = current_streak;
                row.EndEdit();
                mySqlDataAdapter.Update(db, "Habit");
            }
        }

        public static void UpdateLongestStreak(Guid habit_id, int longest_streak)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Habit");
            if (GetHabitByID(habit_id, GetUserId(habit_id)) != null)
            {
                DataRow row = db.Habit.Rows.Find(habit_id);
                row.BeginEdit();
                row["Longest_Streak"] = longest_streak;
                row.EndEdit();
                
                mySqlDataAdapter.Update(db, "Habit");
            }
        }
    }
}
