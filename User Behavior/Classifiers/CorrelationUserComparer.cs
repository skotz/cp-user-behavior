using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class CorrelationUserComparer : IUserComparer
    {
        public double CompareUsers(double[] userFeaturesOne, double[] userFeaturesTwo)
        {
            //double avg1 = userFeaturesOne.Average();
            //double avg2 = userFeaturesTwo.Average();

            //double sum1 = userFeaturesOne.Zip(userFeaturesTwo, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            //double sumSqr1 = userFeaturesOne.Sum(x => Math.Pow((x - avg1), 2.0));
            //double sumSqr2 = userFeaturesTwo.Sum(y => Math.Pow((y - avg2), 2.0));

            //double result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);

            double average1 = 0.0;
            double average2 = 0.0;
            int count = 0;

            for (int i =0; i < userFeaturesOne.Length; i++)
            {
                if (userFeaturesOne[i] != 0 && userFeaturesTwo[i] != 0)
                {
                    average1 += userFeaturesOne[i];
                    average2 += userFeaturesTwo[i];
                    count++;
                }
            }

            average1 /= count;
            average2 /= count;

            double sum = 0.0;

            for (int i = 0; i < userFeaturesOne.Length; i++)
            {
                if (userFeaturesOne[i] != 0 && userFeaturesTwo[i] != 0)
                {
                    sum += (userFeaturesOne[i] - average1) * (userFeaturesTwo[i] - average2);
                }
            }

            double squares1 = 0.0;
            double squares2 = 0.0;

            for (int i = 0; i < userFeaturesOne.Length; i++)
            {
                if (userFeaturesOne[i] != 0 && userFeaturesTwo[i] != 0)
                {
                    squares1 += Math.Pow(userFeaturesOne[i] - average1, 2);
                    squares2 += Math.Pow(userFeaturesTwo[i] - squares2, 2);
                }
            }
            
            return sum / Math.Sqrt(squares1 * squares2);
        }
    }
}
