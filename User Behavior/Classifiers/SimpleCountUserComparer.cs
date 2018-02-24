using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class SimpleCountUserComparer : IUserComparer
    {
        public double CompareUsers(double[] userFeaturesOne, double[] userFeaturesTwo)
        {
            double count = 0.0;
            for (int i = 0; i < userFeaturesOne.Length; i++)
            {
                if (userFeaturesOne[i] != 0 && userFeaturesTwo[i] != 0)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
