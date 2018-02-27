using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Comparers
{
    public class RootMeanSquareUserComparer : IComparer
    {
        public double CompareVectors(double[] userFeaturesOne, double[] userFeaturesTwo)
        {
            double score = 0.0;

            for (int i = 0; i < userFeaturesOne.Length; i++)
            {
                score += Math.Pow(userFeaturesOne[i] - userFeaturesTwo[i], 2);
            }

            // Higher numbers indicate closer similarity
            return -Math.Sqrt(score / userFeaturesOne.Length);
        }
    }
}
