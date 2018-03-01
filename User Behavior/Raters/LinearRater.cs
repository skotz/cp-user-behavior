using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Abstractions;
using UserBehavior.Objects;

namespace UserBehavior.Raters
{
    public class LinearRater : IRater
    {
        private double downVoteWeight;
        private double upVoteWeight;
        private double viewWeight;
        private double downloadWeight;

        private double minWeight;
        private double maxWeight;

        public LinearRater()
            : this (-5.0, 1.0, 3.0, 0.5, 5.0)
        {
        }

        public LinearRater(double downVote, double upvote, double view, double download)
            : this (downVote, upvote, view, download, 5.0)
        {
        }

        public LinearRater(double downVote, double upVote, double view, double download, double max)
        {
            downVoteWeight = downVote;
            upVoteWeight = upVote;
            viewWeight = view;
            downloadWeight = download;

            minWeight = 0.1;
            maxWeight = max;
        }

        public double GetRating(List<UserAction> actions)
        {
            int up = actions.Count(x => x.Action == "UpVote");
            int down = actions.Count(x => x.Action == "DownVote");
            int view = actions.Count(x => x.Action == "View");
            int dl = actions.Count(x => x.Action == "Download");

            double rating = up * upVoteWeight + down * downVoteWeight + view * viewWeight + dl * downloadWeight;

            return Math.Min(maxWeight, Math.Max(minWeight, rating));
        }
    }
}
