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
    }
}
