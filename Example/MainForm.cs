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
using UserBehavior.Abstractions;
using UserBehavior.Comparers;
using UserBehavior.Objects;
using UserBehavior.Parsers;
using UserBehavior.Raters;
using UserBehavior.Recommenders;

namespace Example
{
    public partial class MainForm : Form
    {
        IRecommender recommender;
        string savedModel = "recommender.dat";

        public MainForm()
        {
            InitializeComponent();

            IRater rate = new LinearRater(-4, 2, 3, 1);
            IComparer compare = new CorrelationUserComparer();

            recommender = new UserCollaborativeFilterRecommender(compare, rate, 50);

            if (File.Exists(savedModel))
            {
                try
                {
                    recommender.Load(savedModel);
                    rtbOutput.Text = "Loaded model from file";
                    EnableForm(true);
                }
                catch
                {
                    rtbOutput.Text = "Saved model is corrupt";
                }
            }

            //RecommenderTests.TestAllRecommenders();
            //RecommenderTests.FindBestRaterWeights();
        }

        private void btnLoadTrain_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy && ofdGetUserBehaviors.ShowDialog() == DialogResult.OK)
            {
                rtbOutput.Text = "Training...";
                bgWorker.RunWorkerAsync(ofdGetUserBehaviors.FileName);
                EnableForm(false);
            }
        }
        
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UserBehaviorDatabaseParser parser = new UserBehaviorDatabaseParser();
            UserBehaviorDatabase db = parser.LoadUserBehaviorDatabase(e.Argument as string);

            recommender.Train(db);
            recommender.Save(savedModel);
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            rtbOutput.Text = "Finished training\r\nModel saved to file";
            EnableForm(true);
        }

        private void EnableForm(bool enabled)
        {
            groupRecommend.Enabled = enabled;
            groupScore.Enabled = enabled;
            groupTrain.Enabled = enabled;
        }

        private void btnScore_Click(object sender, EventArgs e)
        {
            if (!bgScore.IsBusy)
            {
                int userId;
                int articleId;
                int.TryParse(txtScoreUser.Text, out userId);
                int.TryParse(txtScoreArticle.Text, out articleId);
                
                if (userId >= 1 && userId <= 3000 && articleId >= 1 && articleId <= 3000)
                {
                    bgScore.RunWorkerAsync(new GetRating { UserID = userId, ArticleID = articleId });
                    EnableForm(false);
                }
                else
                {
                    MessageBox.Show("Invalid User ID or Article ID!");
                }
            }
        }

        private void bgScore_DoWork(object sender, DoWorkEventArgs e)
        {
            GetRating args = e.Argument as GetRating;
            e.Result = recommender.GetRating(args.UserID, args.ArticleID);
        }

        private void bgScore_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            rtbOutput.Text = "Predicted rating: " + ((double)e.Result).ToString("0.00");
            EnableForm(true);
        }

        private void btnRecommend_Click(object sender, EventArgs e)
        {
            if (!bgRecommend.IsBusy)
            {
                int userId;
                int ratings;
                int.TryParse(txtRecommendUser.Text, out userId);
                int.TryParse(txtRecommendNum.Text, out ratings);

                if (userId >= 1 && userId <= 3000 && ratings >= 1 && ratings <= 100)
                {
                    bgRecommend.RunWorkerAsync(new GetRecommendation { UserID = userId, Ratings = ratings });
                    EnableForm(false);
                }
                else
                {
                    MessageBox.Show("Invalid User ID or Recommendation Count!");
                }
            }
        }

        private void bgRecommend_DoWork(object sender, DoWorkEventArgs e)
        {
            GetRecommendation args = e.Argument as GetRecommendation;
            e.Result = recommender.GetSuggestions(args.UserID, args.Ratings);
        }

        private void bgRecommend_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<Suggestion> suggestions = e.Result as List<Suggestion>;

            rtbOutput.Text = "Recommendations:";
            foreach (Suggestion suggestion in suggestions)
            {
                rtbOutput.Text += "\r\n" + suggestion.ArticleID;
            }

            EnableForm(true);
        }
    }

    class GetRating
    {
        public int UserID { get; set; }
        public int ArticleID { get; set; }
    }

    class GetRecommendation
    {
        public int UserID { get; set; }
        public int Ratings { get; set; }
    }
}
