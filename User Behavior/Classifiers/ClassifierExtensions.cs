using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    static class ClassifierExtensions
    {
        public static TestResults Test(this IClassifier classifier, UserBehaviorDatabase db, int numSuggestions)
        {
            int correct = 0;
            int madeSuggestions = 0;

            // Get a list of users in this database who interacted with an article for the first time
            List<int> distinctUsers = db.UserActions.Select(x => x.UserID).Distinct().ToList();
            
            // Now get suggestions for each of these users
            foreach (int user in distinctUsers)
            {
                List<Suggestion> suggestions = classifier.GetSuggestions(user, numSuggestions);
                
                if (suggestions.Count > 0)
                {
                    madeSuggestions++;
                }

                //foreach (var art in db.UserActions.Where(x => x.UserID == user).Select(x => x.ArticleID).Distinct())
                //{
                //    int position = suggestions.FindIndex(x => x.ArticleID == art);
                //}

                foreach (Suggestion s in suggestions)
                {
                    // If one of the top N suggestions is what the user ended up reading, then we're golden
                    if (db.UserActions.Any(x => x.UserID == user && x.ArticleID == s.ArticleID))
                    {
                        correct++;
                        break;
                    }
                }
            }

            return new TestResults(distinctUsers.Count, madeSuggestions, correct);
        }

        public static ScoreResults Score(this IClassifier classifier, UserBehaviorDatabase db)
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
