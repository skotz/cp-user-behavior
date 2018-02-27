using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Objects
{
    public class UserArticleRatingsTable
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

        public void SaveSparcityVisual(string file)
        {
            double min = UserArticleRatings.Min(x => x.ArticleRatings.Min());
            double max = UserArticleRatings.Max(x => x.ArticleRatings.Max());

            Bitmap b = new Bitmap(ArticleIndexToID.Count, UserIndexToID.Count);
            int numPixels = 0;

            for (int x = 0; x < ArticleIndexToID.Count; x++)
            {
                for (int y = 0; y < UserIndexToID.Count; y++)
                {
                    //int brightness = 255 - (int)(((UserArticleRatings[y].ArticleRatings[x] - min) / (max - min)) * 255);
                    //brightness = Math.Max(0, Math.Min(255, brightness));

                    int brightness = UserArticleRatings[y].ArticleRatings[x] == 0 ? 255 : 0;

                    Color c = Color.FromArgb(brightness, brightness, brightness);

                    b.SetPixel(x, y, c);

                    numPixels += UserArticleRatings[y].ArticleRatings[x] != 0 ? 1 : 0;
                }
            }

            double sparcity = (double)numPixels / (ArticleIndexToID.Count * UserIndexToID.Count);

            b.Save(file);
        }

        /// <summary>
        /// Generate a CSV report of users and how many ratings they've given
        /// </summary>
        public void SaveUserRatingDistribution(string file)
        {
            int bucketSize = 2;            
            int maxRatings = UserArticleRatings.Max(x => x.ArticleRatings.Count(y => y != 0));
            List<int> buckets = new List<int>();

            for (int i = 0; i <= Math.Floor((double)maxRatings / bucketSize); i++)
            {
                buckets.Add(0);
            }

            foreach (UserArticleRatings ratings in UserArticleRatings)
            {
                buckets[(int)Math.Floor((double)ratings.ArticleRatings.Count(x => x != 0) / bucketSize)]++;
            }

            using (StreamWriter w = new StreamWriter(file))
            {
                w.WriteLine("numArticlesRead,numUsers");

                for (int i = 0; i <= Math.Floor((double)maxRatings / bucketSize); i++)
                {
                    w.WriteLine("=\"" + (i * bucketSize) + "-" + (((i + 1) * bucketSize) - 1) + "\"," + buckets[i / bucketSize]);
                }
            }
        }

        /// <summary>
        /// Generate a CSV report of articles and how many ratings they've gotten
        /// </summary>
        public void SaveArticleRatingDistribution(string file)
        {
            int bucketSize = 2;
            int maxRatings = 3000;
            List<int> buckets = new List<int>();

            for (int i = 0; i <= Math.Floor((double)maxRatings / bucketSize); i++)
            {
                buckets.Add(0);
            }

            for (int i = 0; i < ArticleIndexToID.Count; i ++)
            {
                int readers = UserArticleRatings.Select(x => x.ArticleRatings[i]).Count(x => x != 0);
                buckets[(int)Math.Floor((double)readers / bucketSize)]++;
            }
            
            while (buckets[buckets.Count - 1] == 0)
            {
                buckets.RemoveAt(buckets.Count - 1);
            }

            using (StreamWriter w = new StreamWriter(file))
            {
                w.WriteLine("numReaders,numArticles");

                for (int i = 0; i < buckets.Count; i++)
                {
                    w.WriteLine("=\"" + (i * bucketSize) + "-" + (((i + 1) * bucketSize) - 1) + "\"," + buckets[i]);
                }
            }
        }
    }
}
