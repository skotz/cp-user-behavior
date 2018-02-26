using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Objects
{
    class ArticleRating
    {
        public int ArticleID { get; set; }

        public double Rating { get; set; }

        public ArticleRating(int articleId, double rating)
        {
            ArticleID = articleId;
            Rating = rating;
        }
    }
}
