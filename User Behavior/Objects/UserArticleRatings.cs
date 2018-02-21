namespace UserBehavior
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

        public static UserArticleRatings operator +(UserArticleRatings uar1, UserArticleRatings uar2)
        {
            UserArticleRatings uar = new UserArticleRatings(0, uar1.ArticleRatings.Length);

            for (int i = 0; i < uar.ArticleRatings.Length; i++)
            {
                uar.ArticleRatings[i] = uar1.ArticleRatings[i] + uar2.ArticleRatings[i];
            }

            return uar;
        }
    }
}
