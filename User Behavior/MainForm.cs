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

namespace UserBehavior
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            UserBehaviorDatabaseParser dbp = new UserBehaviorDatabaseParser();
            UserBehaviorDatabase db = dbp.LoadUserBehaviorDatabase("UserBehaviors.txt");

            ISplitter sp = new DaySplitter(db, 5);
            UserBehaviorDatabase trainDb = sp.GetTrainingDatabase();
            UserBehaviorDatabase testDb = sp.GetTestingDatabase();

            //IUserComparer uc = new RootMeanSquareUserComparer();
            IUserComparer uc = new CoRatedCosineUserComparer();
            UserBehaviorClassifier ubc = new UserBehaviorClassifier(uc);

            //ubc.Train(trainDb);
            //ubc.Save("model-20180220-i.dat");
            ubc.Load("model-20180220-i.dat");

            //UserBehaviorTransformer x = new UserBehaviorTransformer(trainDb);
            //ubc.userArticleRatings = x.GetUserArticleRatings();
            //ubc.Save("model-20180220-h.dat");

            TestResults results = ubc.Test(testDb, 5);

            ubc.GetSuggestions(1, 5);
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
