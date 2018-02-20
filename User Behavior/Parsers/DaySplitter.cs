using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class DaySplitter : ISplitter
    {
        private UserBehaviorDatabase database;
        private int splitDay;

        public DaySplitter(UserBehaviorDatabase db, int daysForTesting)
        {
            database = db;
            splitDay = db.UserActions.Max(x => x.Day) - daysForTesting;
        }

        public UserBehaviorDatabase GetTrainingDatabase()
        {
            UserBehaviorDatabase db = database.Clone();
            db.UserActions.RemoveAll(x => x.Day > splitDay);
            return db;
        }

        public UserBehaviorDatabase GetTestingDatabase()
        {
            UserBehaviorDatabase db = database.Clone();
            db.UserActions.RemoveAll(x => x.Day <= splitDay);
            return db;
        }
    }
}
