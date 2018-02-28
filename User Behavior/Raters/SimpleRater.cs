using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Abstractions;
using UserBehavior.Objects;

namespace UserBehavior.Raters
{
    /// <summary>
    /// Return 0 if the article was downvoted, otherwise provide a rating of 1.
    /// </summary>
    public class SimpleRater : IRater
    {
        public double GetRating(List<UserAction> actions)
        {
            if (actions.Any(x => x.Action == "DownVote"))
            {
                return 0.0;
            }

            return 1.0;
        }
    }
}
