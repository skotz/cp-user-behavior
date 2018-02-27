using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Objects;
using UserBehavior.Parsers;

namespace UserBehavior.Recommenders
{
    public class HybridRecommender : IRecommender
    {
        private List<IRecommender> classifiers;
        
        public HybridRecommender()
        {
            classifiers = new List<IRecommender>();
        }

        public HybridRecommender(params IRecommender[] recommenders)
        {
            classifiers = recommenders.ToList();
        }

        public void Add(IRecommender classifier)
        {
            classifiers.Add(classifier);
        }

        public void Train(UserBehaviorDatabase db)
        {
            foreach (IRecommender classifier in classifiers)
            {
                classifier.Train(db);
            }
        }

        public double GetRating(int userId, int articleId)
        {
            return classifiers.Select(classifier => classifier.GetRating(userId, articleId)).Average();
        }

        public List<Suggestion> GetSuggestions(int userId, int numSuggestions)
        {
            List<Suggestion> suggestions = new List<Suggestion>();
            int numSuggestionsEach = (int)Math.Ceiling((double)numSuggestions / classifiers.Count);

            foreach (IRecommender classifier in classifiers)
            {
                suggestions.AddRange(classifier.GetSuggestions(userId, numSuggestionsEach));
            }

            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return suggestions.Take(numSuggestions).ToList();
        }

        private List<Suggestion> GetCommonSuggestions(int userId, int numSuggestions)
        {
            int internalSuggestions = 100;
            
            List<List<Suggestion>> suggestions = new List<List<Suggestion>>();
            foreach (IRecommender classifier in classifiers)
            {
                suggestions.Add(classifier.GetSuggestions(userId, internalSuggestions));
            }

            List<Tuple<Suggestion, int>> final = new List<Tuple<Suggestion, int>>();

            foreach (List<Suggestion> list in suggestions)
            {
                foreach (Suggestion suggestion in list)
                {
                    int existingIndex = final.FindIndex(x => x.Item1.ArticleID == suggestion.ArticleID);

                    if (existingIndex >= 0)
                    {
                        Suggestion highestRated = final[existingIndex].Item1.Rating > suggestion.Rating ? final[existingIndex].Item1 : suggestion;
                        final[existingIndex] = new Tuple<Suggestion, int>(highestRated, final[existingIndex].Item2 + 1);
                    }
                    else
                    {
                        final.Add(new Tuple<Suggestion, int>(suggestion, 1));
                    }
                }
            }

            final = final.OrderByDescending(x => x.Item2).ThenByDescending(x => x.Item1.Rating).ToList();

            return final.Select(x => x.Item1).Take(numSuggestions).ToList();
        }

        public void Save(string file)
        {
            int n = 1;
            foreach (IRecommender classifier in classifiers)
            {
                classifier.Save(file + ".rm" + (n++));
            }
        }

        public void Load(string file)
        {
            int n = 1;
            foreach (IRecommender classifier in classifiers)
            {
                classifier.Load(file + ".rm" + (n++));
            }
        }
    }
}
