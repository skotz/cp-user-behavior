using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    interface IClassifier
    {
        void Train(UserBehaviorDatabase db);

        TestResults Test(UserBehaviorDatabase db, int numSuggestions);

        List<Suggestion> GetSuggestions(int userId, int numSuggestions);

        void Save(string file);

        void Load(string file);
    }
}
