using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class ScoreResults
    {
        public double RootMeanSquareDifference { get; set; }

        public ScoreResults(double rmsd)
        {
            RootMeanSquareDifference = rmsd;
        }
    }
}
