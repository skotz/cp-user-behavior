using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Objects
{
    public class UserArticleRating
    {
        public int UserID { get; set; }

        public int ArticleID { get; set; }

        public double Rating { get; set; }

        public UserArticleRating(int userId, int articleId, double rating)
        {
            UserID = userId;
            ArticleID = articleId;
            Rating = rating;
        }
    }
}
