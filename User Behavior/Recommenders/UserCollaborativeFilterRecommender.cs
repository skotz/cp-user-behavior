using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Comparers;
using UserBehavior.Mathematics;
using UserBehavior.Objects;
using UserBehavior.Parsers;

namespace UserBehavior.Recommenders
{
    public class UserCollaborativeFilterRecommender : IRecommender
    {
        private IComparer comparer;
        private UserArticleRatingsTable ratings;

        private int neighborCount;
        private int latentUserFeatureCount;

        public UserCollaborativeFilterRecommender(IComparer userComparer, int numberOfNeighbors)
        {
            comparer = userComparer;
            neighborCount = numberOfNeighbors;
            latentUserFeatureCount = 20;
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            ratings = ubt.GetUserArticleRatingsTable();

            SingularValueDecomposition svd = new SingularValueDecomposition(latentUserFeatureCount, 100);
            SvdResult results = svd.FactorizeMatrix(ratings);
            
            ratings.AppendUserFeatures(results.UserFeatures);
        }
        
        public double GetRating(int userId, int articleId)
        {
            UserArticleRatings user = ratings.UserArticleRatings.FirstOrDefault(x => x.UserID == userId);
            List<UserArticleRatings> neighbors = GetNearestNeighbors(user, neighborCount);

            return GetRating(user, neighbors, articleId);
        }

        private double GetRating(UserArticleRatings user, List<UserArticleRatings> neighbors, int articleId)
        {
            int articleIndex = ratings.ArticleIndexToID.IndexOf(articleId);

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
            UserArticleRatings user = ratings.UserArticleRatings.FirstOrDefault(x => x.UserID == userId);
            List<Suggestion> suggestions = new List<Suggestion>();
            int userIndex = ratings.UserIndexToID.IndexOf(userId);

            if (user != null)
            {
                var neighbors = GetNearestNeighbors(user, neighborCount);
                
                for (int articleIndex = 0; articleIndex < ratings.ArticleIndexToID.Count; articleIndex++)
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

                        suggestions.Add(new Suggestion(userId, ratings.ArticleIndexToID[articleIndex], score));
                    }
                }
                
                suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));
            }

            return suggestions.Take(numSuggestions).ToList();
        }

        private List<UserArticleRatings> GetNearestNeighbors(UserArticleRatings user, int numUsers)
        {
            List<UserArticleRatings> neighbors = new List<UserArticleRatings>();

            for (int i = 0; i < ratings.UserArticleRatings.Count; i++)
            {
                if (ratings.UserArticleRatings[i].UserID == user.UserID)
                {
                    ratings.UserArticleRatings[i].Score = double.NegativeInfinity;
                }
                else
                {
                    ratings.UserArticleRatings[i].Score = comparer.CompareVectors(ratings.UserArticleRatings[i].ArticleRatings, user.ArticleRatings);
                }
            }

            var similarUsers = ratings.UserArticleRatings.OrderByDescending(x => x.Score);

            return similarUsers.Take(numUsers).ToList();
        }

        public void Save(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Compress))
            using (StreamWriter w = new StreamWriter(zip))
            {
                w.WriteLine(ratings.UserArticleRatings.Count);
                w.WriteLine(ratings.UserArticleRatings[0].ArticleRatings.Length);
                w.WriteLine(ratings.NumberOfTags);

                foreach (UserArticleRatings t in ratings.UserArticleRatings)
                {
                    w.WriteLine(t.UserID);

                    foreach (double v in t.ArticleRatings)
                    {
                        w.WriteLine(v);
                    }
                }

                w.WriteLine(ratings.UserIndexToID.Count);

                foreach (int i in ratings.UserIndexToID)
                {
                    w.WriteLine(i);
                }

                w.WriteLine(ratings.ArticleIndexToID.Count);

                foreach (int i in ratings.ArticleIndexToID)
                {
                    w.WriteLine(i);
                }
            }
        }

        public void Load(string file)
        {
            ratings = new UserArticleRatingsTable();

            using (FileStream fs = new FileStream(file, FileMode.Open))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Decompress))
            using (StreamReader r = new StreamReader(zip))
            {
                long total = long.Parse(r.ReadLine());
                int features = int.Parse(r.ReadLine());

                int tags = int.Parse(r.ReadLine());
                ratings.NumberOfTags = tags;

                for (long i = 0; i < total; i++)
                {
                    int userId = int.Parse(r.ReadLine());
                    UserArticleRatings uat = new UserArticleRatings(userId, features);

                    for (int x = 0; x < features; x++)
                    {
                        uat.ArticleRatings[x] = double.Parse(r.ReadLine());
                    }

                    ratings.UserArticleRatings.Add(uat);
                }

                total = int.Parse(r.ReadLine());

                for (int i = 0; i < total; i++)
                {
                    ratings.UserIndexToID.Add(int.Parse(r.ReadLine()));
                }

                total = int.Parse(r.ReadLine());

                for (int i = 0; i < total; i++)
                {
                    ratings.ArticleIndexToID.Add(int.Parse(r.ReadLine()));
                }
            }
        }
    }
}
