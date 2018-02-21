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
        private UserArticleRatingsTable RATINGS;
        private List<UserArticleRating> userArticleRatings;

        public UserBehaviorClassifier(IUserComparer userComparer)
        {
            comparer = userComparer;
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            RATINGS = ubt.GetUserArticleRatingsTable();
            userArticleRatings = ubt.GetUserArticleRatings();
        }

        public TestResults Test(UserBehaviorDatabase db, int topN)
        {
            int correct = 0;
            int madeSuggestions = 0;

            // Get a list of users in this database who interacted with an article for the first time
            List<UserAction> testUsers = db.UserActions.Where(x => !userArticleRatings.Any(u => u.UserID == x.UserID && u.ArticleID == x.ArticleID)).ToList();
            List<int> distinctUsers = testUsers.Select(x => x.UserID).Distinct().ToList();

            // Now get suggestions for each of these users
            foreach (int user in distinctUsers)
            {
                List<Suggestion> suggestions = GetSuggestions(user, topN);

                if (suggestions.Count > 0)
                {
                    madeSuggestions++;
                }

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

            return new TestResults(distinctUsers.Count, madeSuggestions, correct);
        }

        public List<Suggestion> GetSuggestions(int userId, int numSuggestions)
        {
            UserArticleRatings user = RATINGS.UserArticleRatings.FirstOrDefault(x => x.UserID == userId);
            List<Suggestion> suggestions = new List<Suggestion>();

            if (user != null)
            {
                for (int i = 0; i < RATINGS.UserArticleRatings.Count; i++)
                {
                    if (RATINGS.UserArticleRatings[i].UserID == userId)
                    {
                        RATINGS.UserArticleRatings[i].Score = double.NegativeInfinity;
                    }
                    else
                    {
                        RATINGS.UserArticleRatings[i].Score = comparer.CompareUsers(RATINGS.UserArticleRatings[i].ArticleRatings, user.ArticleRatings);
                    }
                }

                var similarUsers = RATINGS.UserArticleRatings.OrderByDescending(x => x.Score).ToList();

                int topUsers = 5;

                UserArticleRatings uar = similarUsers[0];
                for (int i = 1; i < topUsers; i++)
                {
                    uar += similarUsers[i];
                }
                
                // For all the articles in the combined similar user
                for (int i = 0; i < uar.ArticleRatings.Length; i++)
                {
                    // If the article was rated by at least one similar user
                    if (uar.ArticleRatings[i] > 0)
                    {
                        // And if the article hasn't already been read by the target user
                        if (!RATINGS.UserArticleRatings.Any(x => x.UserID == userId && x.ArticleRatings[i] != 0))
                        {
                            // Propose it as a suggestion
                            suggestions.Add(new Suggestion(userId, RATINGS.ArticleIndexToID[i], uar.ArticleRatings[i]));
                        }
                    }
                }

                suggestions.Sort((c, n) => n.Assurance.CompareTo(c.Assurance));


                //// Rank every user by how similar their article tag preferences are to the user for which we're getting suggestions.
                //// Then find all the articles each of these users have rated and sort by their user similarity score first and
                //// their article rating second. This should get an ordered list of article recommendations for a given user.
                //var suggestedArticles = RATINGS.UserArticleRatings
                //    .Join(userArticleRatings, t => t.UserID, a => a.UserID, (t, a) => new { t.UserID, t.Score, a.Rating, a.ArticleID })
                //    .OrderBy(x => x.Rating > 0 ? 0 : 1).ThenBy(x => -x.Score).ThenBy(x => -x.Rating).ToList();
                
                //foreach (var article in suggestedArticles)
                //{
                //    // Only collect a specific number of suggestions
                //    if (suggestions.Count >= numSuggestions)
                //    {
                //        break;
                //    }

                //    // Only suggest articles the target user hasn't viewed yet
                //    if (!userArticleRatings.Any(x => x.ArticleID == article.ArticleID && x.UserID == userId))
                //    {
                //        // Don't suggest the same article more than once
                //        if (!suggestions.Any(x => x.ArticleID == article.ArticleID))
                //        {
                //            suggestions.Add(new Suggestion(userId, article.ArticleID, article.Score));
                //        }
                //    }
                //}
            }

            return suggestions.Take(numSuggestions).ToList();
        }

        public void Save(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Compress))
            using (StreamWriter w = new StreamWriter(zip))
            {
                w.WriteLine(RATINGS.UserArticleRatings.Count);
                w.WriteLine(RATINGS.UserArticleRatings[0].ArticleRatings.Length);

                foreach (UserArticleRatings t in RATINGS.UserArticleRatings)
                {
                    w.WriteLine(t.UserID);

                    foreach (double v in t.ArticleRatings)
                    {
                        w.WriteLine(v);
                    }
                }

                w.WriteLine(RATINGS.UserIndexToID.Count);

                foreach (int i in RATINGS.UserIndexToID)
                {
                    w.WriteLine(i);
                }

                w.WriteLine(RATINGS.ArticleIndexToID.Count);

                foreach (int i in RATINGS.ArticleIndexToID)
                {
                    w.WriteLine(i);
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
            RATINGS = new UserArticleRatingsTable();
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
                    UserArticleRatings uat = new UserArticleRatings(userId, features);

                    for (int x = 0; x < features; x++)
                    {
                        uat.ArticleRatings[x] = double.Parse(r.ReadLine());
                    }

                    RATINGS.UserArticleRatings.Add(uat);
                }

                total = int.Parse(r.ReadLine());

                for (int i = 0; i < total; i++)
                {
                    RATINGS.UserIndexToID.Add(int.Parse(r.ReadLine()));
                }

                total = int.Parse(r.ReadLine());

                for (int i = 0; i < total; i++)
                {
                    RATINGS.ArticleIndexToID.Add(int.Parse(r.ReadLine()));
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
