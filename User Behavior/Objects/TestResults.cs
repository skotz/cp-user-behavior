using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Objects
{
    public class TestResults
    {
        public int TotalUsers { get; set; }

        public int TotalArticles { get; set; }

        public int ArticlesSolved { get; set; }

        public int UsersSolved { get; set; }

        public TestResults(int totalUsers, int usersSolved, int totalArticles, int articlesSolved)
        {
            TotalUsers = totalUsers;
            ArticlesSolved = articlesSolved;
            UsersSolved = usersSolved;
            TotalArticles = totalArticles;
        }
    }
}
