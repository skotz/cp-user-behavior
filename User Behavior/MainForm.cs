using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserBehavior.Comparers;
using UserBehavior.Objects;
using UserBehavior.Parsers;
using UserBehavior.Recommenders;

namespace UserBehavior
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            UserBehaviorDatabaseParser dbp = new UserBehaviorDatabaseParser();
            UserBehaviorDatabase db = dbp.LoadUserBehaviorDatabase("UserBehaviour.txt");

            var ubt = new UserBehaviorTransformer(db);
            var uart = ubt.GetUserArticleRatingsTable();
            uart.SaveSparcityVisual("sparcity.bmp");
            uart.SaveUserRatingDistribution("distrib.csv");
            uart.SaveArticleRatingDistribution("distriba.csv");

            ISplitter sp = new DaySplitter(db, 3);

            //IUserComparer uc = new RootMeanSquareUserComparer();
            //IUserComparer uc = new CoRatedCosineUserComparer();
            IComparer uc = new CorrelationUserComparer();
            //IUserComparer uc = new SimpleCountUserComparer();

            UserCollaborativeFilterRecommender ubc = new UserCollaborativeFilterRecommender(uc, 20);

            MatrixFactorizationRecommender sc = new MatrixFactorizationRecommender();

            ItemCollaborativeFilterRecommender icf = new ItemCollaborativeFilterRecommender(uc, 20);

            HybridRecommender hc = new HybridRecommender();
            hc.Add(ubc);
            hc.Add(sc);
            hc.Add(icf);

            hc.Train(sp.TrainingDB);

            //hc.Save("model-20180225-u.dat");
            //hc.Load("model-20180225-u.dat");

            ScoreResults scores = hc.Score(sp.TestingDB);
            TestResults results = hc.Test(sp.TestingDB, 30);

            // Individual tests

            //ubc = new UserCollaborativeFilterRecommender(uc, 20);
            //sc = new MatrixFactorizationRecommender();
            //icf = new ItemCollaborativeFilterRecommender(uc, 20);

            //ubc.Train(sp.TrainingDB);
            //ScoreResults scores2 = ubc.Score(sp.TestingDB);
            //TestResults results2 = ubc.Test(sp.TestingDB, 30);

            //sc.Train(sp.TrainingDB);
            //ScoreResults scores3 = sc.Score(sp.TestingDB);
            //TestResults results3 = sc.Test(sp.TestingDB, 30);

            //icf.Train(sp.TrainingDB);
            //ScoreResults scores4 = icf.Score(sp.TestingDB);
            //TestResults results4 = icf.Test(sp.TestingDB, 30);

            //using (StreamWriter w = new StreamWriter("results.csv"))
            //{
            //    w.WriteLine("model,rmse,users,user-solved,articles,articles-solved");
            //    w.WriteLine("UCF," + scores2.RootMeanSquareDifference + "," + results2.TotalUsers + "," + results2.UsersSolved + "," + results2.TotalArticles + "," + results2.ArticlesSolved);
            //    w.WriteLine("SVD," + scores3.RootMeanSquareDifference + "," + results3.TotalUsers + "," + results3.UsersSolved + "," + results3.TotalArticles + "," + results3.ArticlesSolved);
            //    w.WriteLine("ICF," + scores4.RootMeanSquareDifference + "," + results4.TotalUsers + "," + results4.UsersSolved + "," + results4.TotalArticles + "," + results4.ArticlesSolved);
            //    w.WriteLine("HR," + scores.RootMeanSquareDifference + "," + results.TotalUsers + "," + results.UsersSolved + "," + results.TotalArticles + "," + results.ArticlesSolved);
            //}


            hc.GetSuggestions(1, 5);
        }

        private void btnParseDatabase_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy && ofdGetUserBehaviors.ShowDialog() == DialogResult.OK)
            {
                bgWorker.RunWorkerAsync(ofdGetUserBehaviors.FileName);
                btnParseDatabase.Enabled = false;
            }
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //string file = (string)e.Argument;

            //UserBehaviorDatabaseParser dbp = new UserBehaviorDatabaseParser();
            //UserBehaviorDatabase trainDb = dbp.LoadUserBehaviorDatabase(file);
            //UserBehaviorDatabase testDb = dbp.LoadUserBehaviorDatabase(file);

            //// Split the data into a training set from days 1-29 and a testing set from day 30
            //int lastDay = trainDb.UserActions.Max(x => x.Day);
            //trainDb.UserActions.RemoveAll(x => x.Day == lastDay);
            //testDb.UserActions.RemoveAll(x => x.Day != lastDay);

            //// It's important that we generate the user features off data that's not in the testing set
            //UserBehaviorTransformer trainTransform = new UserBehaviorTransformer(trainDb);
            //trainTransform.WriteArticleFeaturesToFile("article-features.csv");
            //trainTransform.WriteUserArticleTagsToFile("user-features.csv");
            //trainTransform.WriteUserArticleRatingsToFile("user-article-ratings-train.csv");

            //// The testing set contains only data from the last day
            //UserBehaviorTransformer testTransform = new UserBehaviorTransformer(testDb);
            //testTransform.WriteUserArticleRatingsToFile("user-article-ratings-test.csv");
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Finished processing user behaviors!", "User Behavior Parser", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnParseDatabase.Enabled = true;
        }
    }
}
