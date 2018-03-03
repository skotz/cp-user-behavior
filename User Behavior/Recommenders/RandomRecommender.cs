using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Objects;
using UserBehavior.Parsers;

namespace UserBehavior.Recommenders
{
    public class RandomRecommender : IRecommender
    {
        private Random rand;

        public RandomRecommender()
        {
            rand = new Random();
        }
        
        public void Train(UserBehaviorDatabase db)
        {
        }

        public double GetRating(int userId, int articleId)
        {
            return rand.NextDouble() * 5.0;
        }

        public List<Suggestion> GetSuggestions(int userId, int numSuggestions)
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            for (int i = 0; i < numSuggestions; i++)
            {
                suggestions.Add(new Suggestion(userId, rand.Next(1, 3000), rand.NextDouble() * 5.0));
            }

            return suggestions;
        }

        public void Load(string file)
        {
            throw new NotImplementedException();
        }

        public void Save(string file)
        {
            throw new NotImplementedException();
        }
    }
}
