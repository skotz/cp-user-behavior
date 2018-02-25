using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class DaySplitter : ISplitter
    {
        public UserBehaviorDatabase TrainingDB { get; private set; }

        public UserBehaviorDatabase TestingDB { get; private set; }

        public DaySplitter(UserBehaviorDatabase db, int daysForTesting)
        {
            int splitDay = db.UserActions.Max(x => x.Day) - daysForTesting;

            TrainingDB = db.Clone();
            TrainingDB.UserActions.RemoveAll(x => x.Day > splitDay);

            TestingDB = db.Clone();
            TestingDB.UserActions.RemoveAll(x => x.Day <= splitDay);

            // Remove any user-article pairs from the testing set that are also in the training set
            TestingDB.UserActions.RemoveAll(test => TrainingDB.UserActions.Any(train => train.UserID == test.UserID && train.ArticleID == test.ArticleID));
        }
    }
}
