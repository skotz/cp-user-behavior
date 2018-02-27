using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Abstractions;
using UserBehavior.Objects;
using UserBehavior.Parsers;
using UserBehavior.Raters;

namespace UserBehavior.Recommenders
{
    public static class ClassifierExtensions
    {
        public static TestResults Test(this IRecommender classifier, UserBehaviorDatabase db, int numSuggestions)
        {
            int correctArticles = 0;
            int correctUsers = 0;

            // Get a list of users in this database who interacted with an article for the first time
            List<int> distinctUsers = db.UserActions.Select(x => x.UserID).Distinct().ToList();
            
            int distinctUserArticles = db.UserActions.GroupBy(x => new { x.UserID, x.ArticleID }).Count();

            // Now get suggestions for each of these users
            foreach (int user in distinctUsers)
            {
                List<Suggestion> suggestions = classifier.GetSuggestions(user, numSuggestions);
                bool foundOne = false;
                
                foreach (Suggestion s in suggestions)
                {
                    // If one of the top N suggestions is what the user ended up reading, then we're golden
                    if (db.UserActions.Any(x => x.UserID == user && x.ArticleID == s.ArticleID))
                    {
                        correctArticles++;

                        if (!foundOne)
                        {
                            correctUsers++;
                            foundOne = true;
                        }
                    }
                }
            }

            return new TestResults(distinctUsers.Count, correctUsers, distinctUserArticles, correctArticles);
        }

        public static ScoreResults Score(this IRecommender classifier, IRater rater, UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            UserArticleRatingsTable actualRatings = ubt.GetUserArticleRatingsTable(rater);

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
                    double predictedRating = classifier.GetRating(userArticle.Key.UserID, userArticle.Key.ArticleID);

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
    }
}
