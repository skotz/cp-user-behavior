using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior.Mathematics
{
    class Matrix
    {
        /// <summary>
        /// Calculate the dot product between two vectors
        /// </summary>
        public static double GetDotProduct(double[] matrixOne, double[] matrixTwo)
        {
            return matrixOne.Zip(matrixTwo, (a, b) => a * b).Sum();
        }
    }
}
