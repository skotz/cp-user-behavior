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

        public int Suggestions { get; set; }

        public int Correct { get; set; }

        public TestResults(int samples, int suggestions, int correct)
        {
            Samples = samples;
            Suggestions = suggestions;
            Correct = correct;
        }
    }
}
