using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Mathematics;

namespace UserBehavior
{
    class UserBehaviorClassifier : IClassifier
    {
        private IUserComparer comparer;
        private UserArticleRatingsTable RATINGS;

        private int neighborCount;
        private int latentUserFeatureCount;

        public UserBehaviorClassifier(IUserComparer userComparer, int knn)
        {
            comparer = userComparer;
            neighborCount = knn;
            latentUserFeatureCount = 20;
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            RATINGS = ubt.GetUserArticleRatingsTable();

            SingularValueDecomposition svd = new SingularValueDecomposition(latentUserFeatureCount, 100);
            SvdResult results = svd.FactorizeMatrix(RATINGS);
            
            RATINGS.AppendUserFeatures(results.UserFeatures);
        }
        
        public double GetRating(int userId, int articleId)
        {
            UserArticleRatings user = RATINGS.UserArticleRatings.FirstOrDefault(x => x.UserID == userId);
            List<UserArticleRatings> neighbors = GetNearestNeighbors(user, neighborCount);

            return GetRating(user, neighbors, articleId);
        }

        private double GetRating(UserArticleRatings user, List<UserArticleRatings> neighbors, int articleId)
        {
            int articleIndex = RATINGS.ArticleIndexToID.IndexOf(articleId);

            var nonZero = user.ArticleRatings.Where(x => x != 0);
            double avgUserRating = nonZero.Count() > 0 ? nonZero.Average() : 0.0;

            double score = 0.0;
            int count = 0;
            for (int u = 0; u < neighbors.Count; u++)
            {
                var nonZeroRatings = neighbors[u].ArticleRatings.Where(x => x != 0);
                double avgRating = nonZeroRatings.Count() > 0 ? nonZeroRatings.Average() : 0.0;

                if (neighbors[u].ArticleRatings[articleIndex] != 0)
                {
                    score += neighbors[u].ArticleRatings[articleIndex] - avgRating;
                    count++;
                }
            }
            if (count > 0)
            {
                score /= count;
                score += avgUserRating;
            }

            return score;
        }

        public List<Suggestion> GetSuggestions(int userId, int numSuggestions)
        {
            UserArticleRatings user = RATINGS.UserArticleRatings.FirstOrDefault(x => x.UserID == userId);
            List<Suggestion> suggestions = new List<Suggestion>();
            int userIndex = RATINGS.UserIndexToID.IndexOf(userId);

            if (user != null)
            {
                //// Fill in missing ratings
                //for (int articleIndex = 0; articleIndex < similarUsers[userIndex].ArticleRatings.Length; articleIndex++)
                //{
                //    if (similarUsers[userIndex].ArticleRatings[articleIndex] == 0)
                //    {
                //        similarUsers[userIndex].ArticleRatings[articleIndex] = averageRatings.ArticleRatings[articleIndex];
                //    }
                //}


                //for (int articleIndex = 0; articleIndex < RATINGS.ArticleIndexToID.Count; articleIndex++)
                //{
                //    int articleId = RATINGS.ArticleIndexToID[articleIndex];

                //    // Only suggest articles the user hasn't read yet
                //    if (RATINGS.UserArticleRatings[userIndex].ArticleRatings[articleIndex] == 0)
                //    {
                //        double rating = GetRating(user, neighbors, articleId);

                //        suggestions.Add(new Suggestion(userId, articleId, rating));
                //    }
                //}

                //var nonZero = user.ArticleRatings.Where(x => x != 0);
                //double avgUserRating = nonZero.Count() > 0 ? nonZero.Average() : 0.0;

                var neighbors = GetNearestNeighbors(user, neighborCount);
                
                //using (StreamWriter w = new StreamWriter("neighbors-table.csv"))
                //{
                //    //w.Write(user.Score + ",");
                //    for (int a = 0; a < user.ArticleRatings.Length; a++)
                //    {
                //        w.Write(user.ArticleRatings[a] + (a != user.ArticleRatings.Length - 1 ? "," : ""));
                //    }
                //    w.WriteLine();

                //    for (int u = 0; u < neighbors.Count; u++)
                //    {
                //        //w.Write(neighbors[u].Score + ",");
                //        for (int a = 0; a < neighbors[0].ArticleRatings.Length; a++)
                //        {
                //            w.Write(neighbors[u].ArticleRatings[a] + (a != neighbors[0].ArticleRatings.Length - 1 ? "," : ""));
                //        }
                //        w.WriteLine();
                //    }
                //}

                for (int articleIndex = 0; articleIndex < RATINGS.ArticleIndexToID.Count; articleIndex++)
                {
                    // If the user in question hasn't rated the given article yet
                    if (user.ArticleRatings[articleIndex] == 0)
                    {
                        double score = 0.0;
                        int count = 0;
                        for (int u = 0; u < neighbors.Count; u++)
                        {
                            if (neighbors[u].ArticleRatings[articleIndex] != 0)
                            {
                                //var nonZeroRatings = neighbors[u].ArticleRatings.Where(x => x != 0);
                                //double avgRating = nonZeroRatings.Count() > 0 ? nonZeroRatings.Average() : 0.0;

                                // Calculate the weighted score for this article   
                                score += neighbors[u].ArticleRatings[articleIndex]; // - avgRating; // * neighbors[u].Score;
                                count++;
                            }
                        }
                        if (count > 0)
                        {
                            score /= count;
                            //score += avgUserRating;
                        }

                        suggestions.Add(new Suggestion(userId, RATINGS.ArticleIndexToID[articleIndex], score));
                    }
                }

                //// For all the articles in the combined similar user
                //for (int i = 0; i < averageRatings.ArticleRatings.Length; i++)
                //{
                //    // If the article was rated by at least one similar user
                //    if (averageRatings.ArticleRatings[i] > 0)
                //    {
                //        // And if the article hasn't already been read by the target user
                //        if (!RATINGS.UserArticleRatings.Any(x => x.UserID == userId && x.ArticleRatings[i] != 0))
                //        {
                //            // Propose it as a suggestion
                //            suggestions.Add(new Suggestion(userId, RATINGS.ArticleIndexToID[i], averageRatings.ArticleRatings[i]));
                //        }
                //    }
                //}

                suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));


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

        private List<UserArticleRatings> GetNearestNeighbors(UserArticleRatings user, int numUsers)
        {
            List<UserArticleRatings> neighbors = new List<UserArticleRatings>();

            for (int i = 0; i < RATINGS.UserArticleRatings.Count; i++)
            {
                if (RATINGS.UserArticleRatings[i].UserID == user.UserID)
                {
                    RATINGS.UserArticleRatings[i].Score = double.NegativeInfinity;
                }
                else
                {
                    RATINGS.UserArticleRatings[i].Score = comparer.CompareUsers(RATINGS.UserArticleRatings[i].ArticleRatings, user.ArticleRatings);
                }
            }

            var similarUsers = RATINGS.UserArticleRatings.OrderByDescending(x => x.Score);

            //for (int i = 1; i < numUsers; i++)
            //{
            //    neighbors.Add(similarUsers[i]);
            //}

            //// Get the average scores for every item
            //UserArticleRatings averageRatings = similarUsers[0];
            //for (int i = 1; i < numUsers; i++)
            //{
            //    averageRatings += similarUsers[i];
            //}
            //averageRatings /= numUsers;

            return similarUsers.Take(numUsers).ToList();
        }

        public void Save(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Compress))
            using (StreamWriter w = new StreamWriter(zip))
            {
                w.WriteLine(RATINGS.UserArticleRatings.Count);
                w.WriteLine(RATINGS.UserArticleRatings[0].ArticleRatings.Length);
                w.WriteLine(RATINGS.NumberOfTags);

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

                //w.WriteLine(userArticleRatings.Count);

                //foreach (UserArticleRating r in userArticleRatings)
                //{
                //    w.WriteLine(r.UserID);
                //    w.WriteLine(r.ArticleID);
                //    w.WriteLine(r.Rating);
                //}
            }
        }

        public void Load(string file)
        {
            RATINGS = new UserArticleRatingsTable();
            //userArticleRatings = new List<UserArticleRating>();

            using (FileStream fs = new FileStream(file, FileMode.Open))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Decompress))
            using (StreamReader r = new StreamReader(zip))
            {
                long total = long.Parse(r.ReadLine());
                int features = int.Parse(r.ReadLine());

                int tags = int.Parse(r.ReadLine());
                RATINGS.NumberOfTags = tags;

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

                //total = long.Parse(r.ReadLine());

                //for (long i = 0; i < total; i++)
                //{
                //    int userId = int.Parse(r.ReadLine());
                //    int articleId = int.Parse(r.ReadLine());
                //    double rating = double.Parse(r.ReadLine());

                //    userArticleRatings.Add(new UserArticleRating(userId, articleId, rating));
                //}
            }
        }
    }
}
