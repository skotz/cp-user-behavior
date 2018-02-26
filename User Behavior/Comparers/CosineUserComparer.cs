using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Comparers
{
    class CosineUserComparer : IComparer
    {
        public double CompareVectors(double[] userFeaturesOne, double[] userFeaturesTwo)
        {
            double sumProduct = 0.0;
            double sumOneSquared = 0.0;
            double sumTwoSquared = 0.0;

            for (int i = 0; i < userFeaturesOne.Length; i++)
            {
                sumProduct += userFeaturesOne[i] * userFeaturesTwo[i];
                sumOneSquared += Math.Pow(userFeaturesOne[i], 2);
                sumTwoSquared += Math.Pow(userFeaturesTwo[i], 2);
            }

            return sumProduct / (Math.Sqrt(sumOneSquared) * Math.Sqrt(sumTwoSquared));
        }
    }
}
