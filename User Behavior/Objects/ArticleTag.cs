namespace UserBehavior.Objects
{
    public class ArticleTag
    {
        public int ArticleID { get; set; }

        public string TagName { get; set; }

        public ArticleTag(int articleid, string tag)
        {
            ArticleID = articleid;
            TagName = tag;
        }
    }
}
