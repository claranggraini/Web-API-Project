using Abc.HabitTracker.Api.Factory;
using Abc.HabitTracker.Api.Singleton;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Abc.HabitTracker.Api.Repository
{
    public class Badge_AssignmentRepository
    {
        private static HabitTrackerDataSet db = dbSingleton.getInstance();
        private static SqlConnection myConnection = new SqlConnection("Data Source=.;Initial Catalog=HabitTracker;Integrated Security=True");
        private static SqlDataAdapter DataAdapter(string TableName)
        {
            SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter("Select * from " + TableName, myConnection);
            SqlCommandBuilder mySqlCommandBuilder = new SqlCommandBuilder(mySqlDataAdapter);
            mySqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            mySqlDataAdapter.Fill(db, TableName);
            return mySqlDataAdapter;
        }
        public static int GetBadgeID(string badge_name)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Badge");

            foreach (DataRow row in db.Badge.Rows)
            {
                if (row["Name"].Equals(badge_name))
                {
                    return (int)row["Id"];
                }
            }
            return 0;
        }
        public static void SaveBadgeAssignmentData(Guid user_id, string badge_name, DateTime created_at)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Badge_Assignment");

            int badge_id = GetBadgeID(badge_name);

            Badge_Assignment ba =Badge_AssignmentFactory.CreateBadgeAssignment(user_id, GetBadgeByID(badge_id), created_at);
            db.Badge_Assignment.Rows.Add(ba.UserID, badge_id, ba.CreatedAt);
            mySqlDataAdapter.Update(db, "Badge_Assignment");
        }

        public static IEnumerable<Badge_Assignment> GetAssignedBadgeByUserID(Guid user_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Badge_Assignment");
            var AssignedBadges = new List<Badge_Assignment>();
            foreach (DataRow row in db.Badge_Assignment)
            {
                if (row["User_Id"].Equals(user_id))
                {
                    AssignedBadges.Add(new Badge_Assignment(user_id, GetBadgeByID((int)row["Badge_Id"]), DateTime.Now));
                }
            }
            return AssignedBadges;
        }

        private static Badge GetBadgeByID(int badge_id)
        {
            SqlDataAdapter mySqlDataAdapter = DataAdapter("Badge");
            DataRow row = db.Badge.Rows.Find(badge_id);
            Badge b = null;
            b = new Badge(row["Name"].ToString(), row["Description"].ToString());
            return b;
        }
    }
}
