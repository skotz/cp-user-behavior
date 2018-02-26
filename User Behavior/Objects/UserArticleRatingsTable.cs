using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Objects
{
    class UserArticleRatingsTable
    {
        public List<UserArticleRatings> UserArticleRatings { get; set; }

        public List<int> UserIndexToID { get; set; }

        public List<int> ArticleIndexToID { get; set; }

        public int NumberOfTags { get; set; }

        public UserArticleRatingsTable()
        {
            UserArticleRatings = new List<UserArticleRatings>();
            UserIndexToID = new List<int>();
            ArticleIndexToID = new List<int>();
        }
        
        public void AppendUserFeatures(double[][] userFeatures)
        {
            for (int i = 0; i < UserIndexToID.Count; i++)
            {
                UserArticleRatings[i].AppendRatings(userFeatures[i]);
            }
        }

        public void AppendArticleFeatures(double[][] articleFeatures)
        {
            for (int f = 0; f < articleFeatures[0].Length; f++)
            {
                UserArticleRatings newFeature = new UserArticleRatings(int.MaxValue, ArticleIndexToID.Count);
                
                for (int a = 0; a < ArticleIndexToID.Count; a++)
                {
                    newFeature.ArticleRatings[a] = articleFeatures[a][f];
                }

                UserArticleRatings.Add(newFeature);
            }
        }

        internal void AppendArticleFeatures(List<ArticleTagCounts> articleTags)
        {
            double[][] features = new double[articleTags.Count][];

            for (int a = 0; a < articleTags.Count; a++)
            {
                features[a] = new double[articleTags[a].TagCounts.Length];

                for (int f = 0; f < articleTags[a].TagCounts.Length; f++)
                {
                    features[a][f] = articleTags[a].TagCounts[f];
                }
            }

            AppendArticleFeatures(features);
        }

        public Bitmap GenerateVisual()
        {
            double min = UserArticleRatings.Min(x => x.ArticleRatings.Min());
            double max = UserArticleRatings.Max(x => x.ArticleRatings.Max());

            Bitmap b = new Bitmap(ArticleIndexToID.Count, UserIndexToID.Count);

            for (int x = 0; x < ArticleIndexToID.Count; x++)
            {
                for (int y = 0; y < UserIndexToID.Count; y++)
                {
                    //int brightness = 255 - (int)(((UserArticleRatings[y].ArticleRatings[x] - min) / (max - min)) * 255);
                    //brightness = Math.Max(0, Math.Min(255, brightness));

                    int brightness = UserArticleRatings[y].ArticleRatings[x] == 0 ? 255 : 0;

                    Color c = Color.FromArgb(brightness, brightness, brightness);

                    b.SetPixel(x, y, c);
                }
            }

            return b;
        }
    }
}
