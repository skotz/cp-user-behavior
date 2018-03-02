using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserBehavior.Comparers;
using UserBehavior.Objects;
using UserBehavior.Parsers;
using UserBehavior.Raters;
using UserBehavior.Recommenders;

namespace Example
{
    class RecommenderTests
    {
        public static void FindBestRaterWeights()
        {
            using (StreamWriter w = new StreamWriter("rater-weights.csv", true))
            {
                w.WriteLine("down,up,view,dl,rmse,solved,total,precision,rank");
            }

            var once = new User(0, "");
            var options = new List<dynamic>();

            for (double up = 0.0; up < 5.0; up += 1)
            {
                for (double down = -5.0; down <= 0.0; down += 1)
                {
                    for (double dl = 0.0; dl < 5.0; dl += 1)
                    {
                        for (double view = 0.0; view < 5.0; view += 1)
                        {
                            options.Add(new { up, down, dl, view });
                        }
                    }
                }
            }

            var dbp = new UserBehaviorDatabaseParser();
            var db = dbp.LoadUserBehaviorDatabase("UserBehaviour.txt");
            var sp = new DaySplitter(db, 3);
            var cp = new CorrelationUserComparer();
            
            Parallel.ForEach(options, set =>
            {
                try
                {
                    var rate = new LinearRater(set.down, set.up, set.view, set.dl);
                    var mfr = new MatrixFactorizationRecommender(20, rate);
                    //var mfr = new UserCollaborativeFilterRecommender(cp, rate, set.features);

                    mfr.Train(sp.TrainingDB);

                    var score = mfr.Score(sp.TestingDB, rate);
                    var results = mfr.Test(sp.TestingDB, 100);
                    
                    lock (once)
                    {
                        using (StreamWriter w = new StreamWriter("rater-weights.csv", true))
                        {
                            w.WriteLine(set.down + "," + set.up + "," + set.view + "," + set.dl + "," + score.RootMeanSquareDifference + "," + results.UsersSolved + "," + results.TotalUsers + "," + results.AveragePrecision + "," + results.AverageRecall);
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText("errors.txt", ex.ToString());
                }
            });
        }

        public static void TestAllRecommenders()
        {
            UserBehaviorDatabaseParser dbp = new UserBehaviorDatabaseParser();
            UserBehaviorDatabase db = dbp.LoadUserBehaviorDatabase("UserBehaviour.txt");

            //var ubt = new UserBehaviorTransformer(db);
            //var uart = ubt.GetUserArticleRatingsTable();
            //uart.SaveSparcityVisual("sparcity.bmp");
            //uart.SaveUserRatingDistribution("distrib.csv");
            //uart.SaveArticleRatingDistribution("distriba.csv");

            var rate = new LinearRater();
            var sp = new DaySplitter(db, 5);
            var uc = new CorrelationUserComparer();

            var ubc = new UserCollaborativeFilterRecommender(uc, rate, 30);
            var mfr = new MatrixFactorizationRecommender(30, rate);
            var icf = new ItemCollaborativeFilterRecommender(uc, rate, 30);
            var hbr = new HybridRecommender(ubc, mfr, icf);

            hbr.Train(sp.TrainingDB);
            ScoreResults scores1 = hbr.Score(sp.TestingDB, rate);
            TestResults results1 = hbr.Test(sp.TestingDB, 30);

            ubc.Train(sp.TrainingDB);
            ScoreResults scores2 = ubc.Score(sp.TestingDB, rate);
            TestResults results2 = ubc.Test(sp.TestingDB, 30);

            mfr.Train(sp.TrainingDB);
            ScoreResults scores3 = mfr.Score(sp.TestingDB, rate);
            TestResults results3 = mfr.Test(sp.TestingDB, 30);

            icf.Train(sp.TrainingDB);
            ScoreResults scores4 = icf.Score(sp.TestingDB, rate);
            TestResults results4 = icf.Test(sp.TestingDB, 30);

            using (StreamWriter w = new StreamWriter("results.csv"))
            {
                w.WriteLine("model,rmse,users,user-solved,precision,recall");
                w.WriteLine("UCF," + scores2.RootMeanSquareDifference + "," + results2.TotalUsers + "," + results2.UsersSolved + "," + results2.AveragePrecision + "," + results2.AverageRecall);
                w.WriteLine("SVD," + scores3.RootMeanSquareDifference + "," + results3.TotalUsers + "," + results3.UsersSolved + "," + results3.AveragePrecision + "," + results3.AverageRecall);
                w.WriteLine("ICF," + scores4.RootMeanSquareDifference + "," + results4.TotalUsers + "," + results4.UsersSolved + "," + results4.AveragePrecision + "," + results4.AverageRecall);
                w.WriteLine("HR," + scores1.RootMeanSquareDifference + "," + results1.TotalUsers + "," + results1.UsersSolved + "," + results1.AveragePrecision + "," + results1.AverageRecall);
            }
        }
    }
}
