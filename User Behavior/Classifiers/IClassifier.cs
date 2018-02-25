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
        
        List<Suggestion> GetSuggestions(int userId, int numSuggestions);

        double GetRating(int userId, int articleId);
        
        void Save(string file);

        void Load(string file);
    }
}
