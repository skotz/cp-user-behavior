using System;

namespace UserBehavior.Objects
{
    [Serializable]
    public class UserAction
    {
        public int Day { get; set; }

        public string Action { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public int ArticleID { get; set; }

        public string ArticleName { get; set; }

        public UserAction(int day, string action, int userid, string username, int articleid, string articlename)
        {
            Day = day;
            Action = action;
            UserID = userid;
            UserName = username;
            ArticleID = articleid;
            ArticleName = articlename;
        }

        public override string ToString()
        {
            return Day + "," + Action + "," + UserID + "," + UserName + "," + ArticleID + "," + ArticleName;
        }
    }
}
