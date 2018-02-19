namespace UserBehavior
{
    class Suggestion
    {
        public int UserID { get; set; }

        public int ArticleID { get; set; }

        public double Assurance { get; set; }

        public Suggestion(int userId, int articleId, double assurance)
        {
            UserID = userId;
            ArticleID = articleId;
            Assurance = assurance;
        }
    }
}
