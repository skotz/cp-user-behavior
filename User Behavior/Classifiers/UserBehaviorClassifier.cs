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
        private List<UserArticleRating> userArticleRatings;

        public UserBehaviorClassifier(IUserComparer userComparer)
        {
            comparer = userComparer;
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            userActionTags = ubt.GetUserActionTags();
            userArticleRatings = ubt.GetUserArticleRatings();
        }

        public TestResults Test(UserBehaviorDatabase db, int topN)
        {
            int correct = 0;

            // Get a list of users in this database who interacted with an article for the first time
            List<UserAction> testUsers = db.UserActions.Where(x => !userArticleRatings.Any(u => u.UserID == x.UserID && u.ArticleID == x.ArticleID)).ToList();
            List<int> distinctUsers = testUsers.Select(x => x.UserID).Distinct().ToList();

            // Now get suggestions for each of these users
            foreach (int user in distinctUsers)
            {
                List<Suggestion> suggestions = GetSuggestions(user, topN);
                foreach (Suggestion s in suggestions)
                {
                    // If one of the top N suggestions is what the user ended up reading, then we're golden!
                    if (testUsers.Any(x => x.UserID == user && x.ArticleID == s.ArticleID))
                    {
                        correct++;
                        break;
                    }
                }
            }

            return new TestResults(distinctUsers.Count, correct);
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

                // Rank every user by how similar their article tag preferences are to the user for which we're getting suggestions.
                // Then find all the articles each of these users have rated and sort by their user similarity score first and
                // their article rating second. This should get an ordered list of article recommendations for a given user.
                var suggestedArticles = userActionTags
                    .Join(userArticleRatings, t => t.UserID, a => a.UserID, (t, a) => new { t.UserID, t.Score, a.Rating, a.ArticleID })
                    .OrderBy(x => x.Score).ThenBy(x => -x.Rating).ToList();
                
                foreach (var article in suggestedArticles)
                {
                    // Only collect a specific number of suggestions
                    if (suggestions.Count >= numSuggestions)
                    {
                        break;
                    }
                    
                    // Only suggest articles the target user hasn't viewed yet
                    if (!userArticleRatings.Any(x => x.ArticleID == article.ArticleID && x.UserID == userId))
                    {
                        // Don't suggest the same article more than once
                        if (!suggestions.Any(x => x.ArticleID == article.ArticleID))
                        {
                            suggestions.Add(new Suggestion(userId, article.ArticleID, article.Score));
                        }
                    }
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

                w.WriteLine(userArticleRatings.Count);

                foreach (UserArticleRating r in userArticleRatings)
                {
                    w.WriteLine(r.UserID);
                    w.WriteLine(r.ArticleID);
                    w.WriteLine(r.Rating);
                }
            }
        }

        public void Load(string file)
        {
            userActionTags = new List<UserActionTag>();
            userArticleRatings = new List<UserArticleRating>();

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

                total = long.Parse(r.ReadLine());

                for (long i = 0; i < total; i++)
                {
                    int userId = int.Parse(r.ReadLine());
                    int articleId = int.Parse(r.ReadLine());
                    double rating = double.Parse(r.ReadLine());

                    userArticleRatings.Add(new UserArticleRating(userId, articleId, rating));
                }
            }
        }
    }
}
