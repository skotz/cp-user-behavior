using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class UserBehaviorClassifier : IClassifier
    {
        private IUserComparer comparer;
        private List<UserActionTag> userActionTags;
        private List<Article> userActions;

        public UserBehaviorClassifier(IUserComparer userComparer)
        {
            comparer = userComparer;
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            userActionTags = ubt.GetUserActionTags();
        }

        public List<Suggestion> GetSuggestions(int userId, int numSuggestions)
        {
            UserActionTag user = userActionTags.FirstOrDefault(x => x.UserID == userId);
            List<Suggestion> suggestions = new List<Suggestion>();
            List<int> articles = new List<int>();

            if (user != null)
            {
                for (int i = 0; i < userActionTags.Count; i++)
                {
                    if (userActionTags[i].UserID == userId)
                    {
                        userActionTags[i].Score = double.MaxValue;
                    }
                    else
                    {
                        userActionTags[i].Score = comparer.CompareUsers(userActionTags[i].ActionTagData, user.ActionTagData);
                    }
                }

                userActionTags.Sort((c, n) => c.Score.CompareTo(n.Score));
                
                // from the similar users, look up the articles they upvoted, compare them against the articles already viewed by the user, and then present them as suggestions
                // probably need to serialize the database to a file as well

                for (int i = 0; i < numSuggestions; i++)
                {
                    
                }
            }

            return suggestions;
        }

        public void Save(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Compress))
            using (StreamWriter w = new StreamWriter(zip))
            {
                w.WriteLine(userActionTags.Count);
                w.WriteLine(userActionTags[0].ActionTagData.Length);

                foreach (UserActionTag t in userActionTags)
                {
                    w.WriteLine(t.UserID);

                    foreach (double v in t.ActionTagData)
                    {
                        w.WriteLine(v);
                    }
                }
            }
        }

        public void Load(string file)
        {
            userActionTags = new List<UserActionTag>();

            using (FileStream fs = new FileStream(file, FileMode.Open))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Decompress))
            using (StreamReader r = new StreamReader(zip))
            {
                long total = long.Parse(r.ReadLine());
                int features = int.Parse(r.ReadLine());

                for (long i = 0; i < total; i++)
                {
                    int userId = int.Parse(r.ReadLine());
                    UserActionTag uat = new UserActionTag(userId, features);

                    for (int x = 0; x < features; x++)
                    {
                        uat.ActionTagData[x] = double.Parse(r.ReadLine());
                    }

                    userActionTags.Add(uat);
                }
            }
        }
    }
}
