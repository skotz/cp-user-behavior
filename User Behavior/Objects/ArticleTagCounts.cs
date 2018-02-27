using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Objects
{
    public class ArticleTagCounts
    {
        public int ArticleID { get; set; }

        public double[] TagCounts { get; set; }

        public ArticleTagCounts(int articleId, int numTags)
        {
            ArticleID = articleId;
            TagCounts = new double[numTags];
        }
    }
}
