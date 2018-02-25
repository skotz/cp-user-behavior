using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class SvdClassifier : IClassifier
    {
        private UserArticleRatingsTable RATINGS;

        private int numUsers;
        private int numArticles;
        private int numFeatures = 100;

        private double averageGlobalRating;

        private double learningRate = 0.005;
        private double regularizationTerm = 0.02;

        private double[] userBiases;
        private double[] articleBiases;
        private double[][] userFeatures;
        private double[][] articleFeatures;

        public void Train(UserBehaviorDatabase db)
        {
            InitializeRatings(db);
            
            double rmse;
            int count;
            List<double> rmseAll = new List<double>();

            averageGlobalRating = GetAverageRating();
            
            for (int i = 0; i < 100; i++)
            {
                rmse = 0.0;
                count = 0;

                for (int userIndex = 0; userIndex < numUsers; userIndex++)
                {
                    for (int articleIndex = 0; articleIndex < numArticles; articleIndex++)
                    {
                        if (RATINGS.UserArticleRatings[userIndex].ArticleRatings[articleIndex] != 0)
                        {
                            double estimatedRating = averageGlobalRating + userBiases[userIndex] + articleBiases[articleIndex] + GetDotProduct(userFeatures[userIndex], articleFeatures[articleIndex]);
                            
                            double error = RATINGS.UserArticleRatings[userIndex].ArticleRatings[articleIndex] - estimatedRating;
                            
                            rmse += Math.Pow(error, 2);
                            count++;
                            
                            averageGlobalRating += learningRate * (error - regularizationTerm * averageGlobalRating);
                            userBiases[userIndex] += learningRate * (error - regularizationTerm * userBiases[userIndex]);
                            articleBiases[articleIndex] += learningRate * (error - regularizationTerm * articleBiases[articleIndex]);
                            
                            for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                            {
                                userFeatures[userIndex][featureIndex] += learningRate * (error * articleFeatures[articleIndex][featureIndex] - regularizationTerm * userFeatures[userIndex][featureIndex]);
                                articleFeatures[articleIndex][featureIndex] += learningRate * (error * userFeatures[userIndex][featureIndex] - regularizationTerm * articleFeatures[articleIndex][featureIndex]);
                            }
                        }
                    }
                }
                
                rmse = Math.Sqrt(rmse / count);
                rmseAll.Add(rmse);
                
                learningRate *= 0.99;
            }

            using (StreamWriter w = new StreamWriter("rmse.csv"))
            {
                w.WriteLine("epoc,rmse");
                for (int i = 0; i < rmseAll.Count; i++)
                {
                    w.WriteLine((i + 1) + "," + rmseAll[i]);
                }
            }
        }

        /// <summary>
        /// Get the average rating across the entire user-article matrix
        /// </summary>
        private double GetAverageRating()
        {
            double sum = 0.0;
            int count = 0;
            
            for (int userIndex = 0; userIndex < numUsers; userIndex++)
            {
                for (int articleIndex = 0; articleIndex < numArticles; articleIndex++)
                {
                    // If the given user rated the given item, add it to our average
                    if (RATINGS.UserArticleRatings[userIndex].ArticleRatings[articleIndex] != 0)
                    {
                        sum += RATINGS.UserArticleRatings[userIndex].ArticleRatings[articleIndex];
                        count++;
                    }
                }
            }

            return sum / count;
        }

        private double GetDotProduct(double[] matrixOne, double[] matrixTwo)
        {
            return matrixOne.Zip(matrixTwo, (a, b) => a * b).Sum();
        }

        private void InitializeRatings(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            RATINGS = ubt.GetUserArticleRatingsTable();
            numUsers = RATINGS.UserArticleRatings.Count;
            numArticles = RATINGS.UserArticleRatings[0].ArticleRatings.Length;

            Random rand = new Random();

            userFeatures = new double[numUsers][];
            for (int userIndex = 0; userIndex < numUsers; userIndex++)
            {
                userFeatures[userIndex] = new double[numFeatures];

                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    userFeatures[userIndex][featureIndex] = rand.NextDouble();
                }
            }

            articleFeatures = new double[numArticles][];
            for (int articleIndex = 0; articleIndex < numUsers; articleIndex++)
            {
                articleFeatures[articleIndex] = new double[numFeatures];

                for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                {
                    articleFeatures[articleIndex][featureIndex] = rand.NextDouble();
                }
            }

            userBiases = new double[numUsers];
            articleBiases = new double[numArticles];
        }

        public double GetRating(int userId, int articleId)
        {
            int userIndex = RATINGS.UserIndexToID.IndexOf(userId);
            int articleIndex = RATINGS.ArticleIndexToID.IndexOf(articleId);
            
            return averageGlobalRating + userBiases[userIndex] + articleBiases[articleIndex] + GetDotProduct(userFeatures[userIndex], articleFeatures[articleIndex]);
        }

        public ScoreResults Score(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            UserArticleRatingsTable actualRatings = ubt.GetUserArticleRatingsTable();

            var distinctUserArticlePairs = db.UserActions.GroupBy(x => new { x.UserID, x.ArticleID }).ToList();

            double score = 0.0;
            int count = 0;

            foreach (var userArticle in distinctUserArticlePairs)
            {
                int userIndex = actualRatings.UserIndexToID.IndexOf(userArticle.Key.UserID);
                int articleIndex = actualRatings.ArticleIndexToID.IndexOf(userArticle.Key.ArticleID);

                double actualRating = actualRatings.UserArticleRatings[userIndex].ArticleRatings[articleIndex];

                if (actualRating != 0)
                {
                    double predictedRating = GetRating(userArticle.Key.UserID, userArticle.Key.ArticleID);

                    score += Math.Pow(predictedRating - actualRating, 2);
                    count++;
                }
            }

            if (count > 0)
            {
                score = Math.Sqrt(score / count);
            }

            return new ScoreResults(score);
        }

        public List<Suggestion> GetSuggestions(int userId, int numSuggestions)
        {
            UserArticleRatings user = RATINGS.UserArticleRatings.FirstOrDefault(x => x.UserID == userId);
            List<Suggestion> suggestions = new List<Suggestion>();
            int userIndex = RATINGS.UserIndexToID.IndexOf(userId);

            if (user != null)
            {
                for (int articleIndex = 0; articleIndex < RATINGS.ArticleIndexToID.Count; articleIndex++)
                {
                    int articleId = RATINGS.ArticleIndexToID[articleIndex];

                    // If the user in question hasn't rated the given article yet
                    if (user.ArticleRatings[articleIndex] == 0)
                    {
                        double rating = GetRating(user.UserID, articleId);

                        suggestions.Add(new Suggestion(userId, articleId, rating));
                    }
                }

                suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));
            }

            return suggestions.Take(numSuggestions).ToList();
        }

        public TestResults Test(UserBehaviorDatabase db, int numSuggestions)
        {
            int correct = 0;
            int madeSuggestions = 0;

            // Get a list of users in this database who interacted with an article for the first time
            List<UserAction> testUsers = db.UserActions.Where(x => !RATINGS.UserArticleRatings.Any(u => u.UserID == x.UserID && u.ArticleRatings[RATINGS.ArticleIndexToID.IndexOf(x.ArticleID)] != 0)).ToList();
            List<int> distinctUsers = testUsers.Select(x => x.UserID).Distinct().ToList();

            //UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            //UserArticleRatingsTable TEST = ubt.GetUserArticleRatingsTable();
            //double averageArticlesPerUserTrain = RATINGS.UserArticleRatings.Select(u => u.ArticleRatings.Count(a => a != 0)).Average();
            //double averageArticlesPerUserTest = TEST.UserArticleRatings.Select(u => u.ArticleRatings.Count(a => a != 0)).Average();

            // Now get suggestions for each of these users
            foreach (int user in distinctUsers)
            {
                List<Suggestion> suggestions = GetSuggestions(user, numSuggestions);

                if (suggestions.Count > 0)
                {
                    madeSuggestions++;
                }

                foreach (var art in testUsers.Where(x => x.UserID == user).Select(x => x.ArticleID).Distinct())
                {
                    int position = suggestions.FindIndex(x => x.ArticleID == art);
                }

                foreach (Suggestion s in suggestions)
                {
                    // If one of the top N suggestions is what the user ended up reading, then we're golden
                    if (testUsers.Any(x => x.UserID == user && x.ArticleID == s.ArticleID))
                    {
                        correct++;
                        break;
                    }
                }
            }

            return new TestResults(distinctUsers.Count, madeSuggestions, correct);
        }

        public void Save(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Compress))
            using (StreamWriter w = new StreamWriter(zip))
            {
                w.WriteLine(numUsers);
                w.WriteLine(numArticles);
                w.WriteLine(numFeatures);

                w.WriteLine(averageGlobalRating);

                for (int userIndex = 0; userIndex < numUsers; userIndex++)
                {
                    w.WriteLine(userBiases[userIndex]);
                }

                for (int articleIndex = 0; articleIndex < numArticles; articleIndex++)
                {
                    w.WriteLine(articleBiases[articleIndex]);
                }

                for (int userIndex = 0; userIndex < numUsers; userIndex++)
                {
                    for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                    {
                        w.WriteLine(userFeatures[userIndex][featureIndex]);
                    }
                }
                
                for (int articleIndex = 0; articleIndex < numUsers; articleIndex++)
                {
                    for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                    {
                        w.WriteLine(articleFeatures[articleIndex][featureIndex]);
                    }
                }

                foreach (UserArticleRatings t in RATINGS.UserArticleRatings)
                {
                    w.WriteLine(t.UserID);

                    foreach (double v in t.ArticleRatings)
                    {
                        w.WriteLine(v);
                    }
                }
                
                foreach (int i in RATINGS.UserIndexToID)
                {
                    w.WriteLine(i);
                }
                
                foreach (int i in RATINGS.ArticleIndexToID)
                {
                    w.WriteLine(i);
                }
            }
        }

        public void Load(string file)
        {
            RATINGS = new UserArticleRatingsTable();
            
            using (FileStream fs = new FileStream(file, FileMode.Open))
            using (GZipStream zip = new GZipStream(fs, CompressionMode.Decompress))
            using (StreamReader r = new StreamReader(zip))
            {
                numUsers = int.Parse(r.ReadLine());
                numArticles = int.Parse(r.ReadLine());
                numFeatures = int.Parse(r.ReadLine());

                averageGlobalRating = double.Parse(r.ReadLine());

                userBiases = new double[numUsers];
                for (int userIndex = 0; userIndex < numUsers; userIndex++)
                {
                    userBiases[userIndex] = double.Parse(r.ReadLine());
                }

                articleBiases = new double[numArticles];
                for (int articleIndex = 0; articleIndex < numArticles; articleIndex++)
                {
                    articleBiases[articleIndex] = double.Parse(r.ReadLine());
                }

                userFeatures = new double[numUsers][];
                for (int userIndex = 0; userIndex < numUsers; userIndex++)
                {
                    userFeatures[userIndex] = new double[numFeatures];

                    for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                    {
                        userFeatures[userIndex][featureIndex] = double.Parse(r.ReadLine());
                    }
                }

                articleFeatures = new double[numArticles][];
                for (int articleIndex = 0; articleIndex < numUsers; articleIndex++)
                {
                    articleFeatures[articleIndex] = new double[numFeatures];

                    for (int featureIndex = 0; featureIndex < numFeatures; featureIndex++)
                    {
                        articleFeatures[articleIndex][featureIndex] = double.Parse(r.ReadLine());
                    }
                }

                for (int i = 0; i < numUsers; i++)
                {
                    int userId = int.Parse(r.ReadLine());
                    UserArticleRatings uat = new UserArticleRatings(userId, numArticles);

                    for (int x = 0; x < numArticles; x++)
                    {
                        uat.ArticleRatings[x] = double.Parse(r.ReadLine());
                    }

                    RATINGS.UserArticleRatings.Add(uat);
                }

                for (int i = 0; i < numUsers; i++)
                {
                    RATINGS.UserIndexToID.Add(int.Parse(r.ReadLine()));
                }
                
                for (int i = 0; i < numArticles; i++)
                {
                    RATINGS.ArticleIndexToID.Add(int.Parse(r.ReadLine()));
                }
            }
        }
    }
}
