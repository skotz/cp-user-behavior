using System;
using System.Collections.Generic;

namespace UserBehavior.Objects
{
    class UserArticleRatings
    {
        public int UserID { get; set; }

        public double[] ArticleRatings { get; set; }

        public double Score { get; set; }

        public UserArticleRatings(int userId, int articlesCount)
        {
            UserID = userId;
            ArticleRatings = new double[articlesCount];
        }

        public void AppendRatings(double[] ratings)
        {
            List<double> allRatings = new List<double>();

            allRatings.AddRange(ArticleRatings);
            allRatings.AddRange(ratings);

            ArticleRatings = allRatings.ToArray();
        }
    }
}
