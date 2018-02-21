using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class UserArticleRatingsTable
    {
        public List<UserArticleRatings> UserArticleRatings { get; set; }

        public List<int> UserIndexToID { get; set; }

        public List<int> ArticleIndexToID { get; set; }

        public UserArticleRatingsTable()
        {
            UserArticleRatings = new List<UserArticleRatings>();
            UserIndexToID = new List<int>();
            ArticleIndexToID = new List<int>();
        }
    }
}
