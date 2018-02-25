using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class TestResults
    {
        public int Samples { get; set; }
        
        public int Correct { get; set; }

        public TestResults(int samples, int correct)
        {
            Samples = samples;
            Correct = correct;
        }
    }
}
