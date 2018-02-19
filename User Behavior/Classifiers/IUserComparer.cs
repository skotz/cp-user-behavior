using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    interface IUserComparer
    {
        double CompareUsers(double[] userFeaturesOne, double[] userFeaturesTwo);
    }
}
