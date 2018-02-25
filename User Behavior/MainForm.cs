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

            DaySplitter sp = new DaySplitter(db, 2);

            //IUserComparer uc = new RootMeanSquareUserComparer();
            //IUserComparer uc = new CoRatedCosineUserComparer();
            IUserComparer uc = new CorrelationUserComparer();
            //IUserComparer uc = new SimpleCountUserComparer();
            UserCollaborativeFilterRecommender ubc = new UserCollaborativeFilterRecommender(uc, 20);

            MatrixFactorizationRecommender sc = new MatrixFactorizationRecommender();

            HybridRecommender hc = new HybridRecommender();
            hc.Add(ubc);
            hc.Add(sc);

            hc.Train(sp.TrainingDB);
            hc.Save("model-20180225-u.dat");
            //hc.Load("model-20180225-u.dat");

            //UserBehaviorTransformer x = new UserBehaviorTransformer(trainDb);
            //ubc.userArticleRatings = x.GetUserArticleRatings();
            //ubc.Save("model-20180220-h.dat");

            // ScoreResults scores = hc.Score(sp.TestingDB);

            TestResults results = hc.Test(sp.TestingDB, 100);

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
