using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class UserBehaviorTransformer
    {
        private UserBehaviorDatabase db;

        public UserBehaviorTransformer(UserBehaviorDatabase database)
        {
            db = database;
        }

        public void WriteArticleFeaturesToFile(string file)
        {            
            // Create a table of all articles as rows and all tags as columns
            using (StreamWriter w = new StreamWriter(file))
            {
                // Write headers
                w.Write("articleId,articleName");
                foreach (Tag tag in db.Tags)
                {
                    w.Write("," + tag.Name.ToLower().Replace(" ", ""));
                }
                w.WriteLine();

                // Write articles
                foreach (Article article in db.Articles)
                {
                    w.Write(article.ArticleID + "," + article.Name);
                    foreach (Tag tag in db.Tags)
                    {
                        w.Write("," + article.Tags.Any(x => x.Name == tag.Name).ToString().ToLower());
                    }
                    w.WriteLine();
                }
            }
        }

        public void WriteUserFeaturesToFile(string file)
        {
            // Create a table of all users 
            using (StreamWriter w = new StreamWriter(file))
            {
                w.WriteLine("userId,userName");
                foreach (User user in db.Users)
                {
                    w.WriteLine(user.UserID + "," + user.Name);
                }
            }
        }

        public void WriteUserArticleRatingsToFile(string file)
        {
            // Create a table of users and their ratings on articles 
            using (StreamWriter w = new StreamWriter(file))
            {
                var userArticleRatings = db.UserActions
                    .GroupBy(x => new { x.UserID, x.ArticleID })
                    .Select(g => new { g.Key.UserID, g.Key.ArticleID, Rating = GetRating(g) })
                    .OrderBy(x => x.UserID).ThenBy(x => x.ArticleID)
                    .ToList();

                w.WriteLine("userId,articleId,rating");
                foreach (var rating in userArticleRatings)
                {
                    w.WriteLine(rating.UserID + "," + rating.ArticleID + "," + rating.Rating);
                }
            }
        }

        private string GetCategory(IGrouping<object, UserAction> actions)
        {
            return actions.Select(x => x.Action.Replace("DownVote", "X")[0].ToString()).Distinct().OrderBy(x => x).Aggregate((c, n) => c + n);
        }

        private int GetRating(IGrouping<object, UserAction> actions)
        {
            int rating = 0;

            if (actions.Any(x => x.Action == "DownVote"))
            {
                rating = 0;
            }
            else
            {
                if (actions.Any(x => x.Action == "View"))
                {
                    rating += 1;
                }

                if (actions.Any(x => x.Action == "UpVote"))
                {
                    rating += 2;
                }

                if (actions.Any(x => x.Action == "Download"))
                {
                    rating += 1;
                }
            }

            return rating;
        }

        public List<UserActionTag> GetUserActionTags()
        {
            List<UserActionTag> userData = new List<UserActionTag>();

            // Create a table of all users as rows and all action-tag pairs as columns
            // There will be a total of 122 columns (2 + numTags * numActions)
            List<string> uniqueActions = db.UserActions.Select(x => x.Action).Distinct().OrderBy(x => x).ToList();
            List<ArticleTag> articleTags = db.GetArticleTagLinkingTable();

            int numUniqueActions = uniqueActions.Count;
            int numUniqueTags = db.Tags.Count;
            int numFeatures = numUniqueActions * numUniqueTags;

            // Create a dataset that links every user action to a list of tags associated with the article that action was for, then 
            // group them by user, action, and tag so we can get a count of the number of times each user performed a action on an article with a specific tag
            var userActionTags = db.UserActions
                .Join(articleTags, u => u.ArticleID, t => t.ArticleID, (u, t) => new { u.UserID, u.UserName, u.Action, t.TagName })
                .GroupBy(x => new { x.UserID, x.UserName, x.Action, x.TagName })
                .Select(g => new { g.Key.UserID, g.Key.UserName, g.Key.Action, g.Key.TagName, Count = g.Count() })
                .OrderBy(x => x.UserID).ThenBy(x => x.Action).ThenBy(x => x.TagName)
                .ToList();

            int totalUserActions = userActionTags.Count();
            int lastFoundIndex = 0;

            // Write action-tag data
            // Parallel.ForEach(db.Users, user =>
            foreach (User user in db.Users)
            {
                int dataCol = 0;
                UserActionTag uat = new UserActionTag(user.UserID, numFeatures);

                foreach (Tag tag in db.Tags)
                {
                    foreach (string action in uniqueActions)
                    {
                        // Count the number of times this user performed this action on an article with this tag
                        // We can loop through like this since the list is sorted
                        int tagActionCount = 0;
                        for (int i = lastFoundIndex; i < totalUserActions; i++)
                        {
                            if (userActionTags[i].UserID == user.UserID && userActionTags[i].Action == action && userActionTags[i].TagName == tag.Name)
                            {
                                lastFoundIndex = i;
                                tagActionCount = userActionTags[i].Count;
                                break;
                            }
                        }

                        uat.ActionTagData[dataCol++] = tagActionCount;
                    }
                }

                // Normalize data to values between 0 and 1
                double max = uat.ActionTagData.Max();
                if (max > 0)
                {
                    for (int i = 0; i < uat.ActionTagData.Length; i++)
                    {
                        uat.ActionTagData[i] /= max;
                    }
                }

                userData.Add(uat);
            }

            return userData;
        }

        public void WriteUserArticleTagsToFile(string file)
        {
            // Create a table of all users as rows and all action-tag pairs as columns
            // There will be a total of 122 columns (2 + numTags * numActions)
            List<string> uniqueActions = db.UserActions.Select(x => x.Action).Distinct().OrderBy(x => x).ToList();
            using (StreamWriter w = new StreamWriter(file))
            {
                // Write headers
                w.Write("userId,userName");
                foreach (Tag tag in db.Tags)
                {
                    foreach (string action in uniqueActions)
                    {
                        w.Write("," + tag.Name.Replace(" ", "") + action.Replace(" ", ""));
                    }
                }
                w.WriteLine();

                List<ArticleTag> articleTags = db.GetArticleTagLinkingTable();

                // Create a dataset that links every user action to a list of tags associated with the article that action was for, then 
                // group them by user, action, and tag so we can get a count of the number of times each user performed a action on an article with a specific tag
                var userActionTags = db.UserActions
                    .Join(articleTags, u => u.ArticleID, t => t.ArticleID, (u, t) => new { u.UserID, u.UserName, u.Action, t.TagName })
                    .GroupBy(x => new { x.UserID, x.UserName, x.Action, x.TagName })
                    .Select(g => new { g.Key.UserID, g.Key.UserName, g.Key.Action, g.Key.TagName, Count = g.Count() })
                    .OrderBy(x => x.UserID).ThenBy(x => x.Action).ThenBy(x => x.TagName)
                    .ToList();

                int totalUserActions = userActionTags.Count();
                int lastFoundIndex = 0;

                // Write action-tag data
                foreach (User user in db.Users)
                {
                    w.Write(user.UserID + "," + user.Name);
                    foreach (Tag tag in db.Tags)
                    {
                        foreach (string action in uniqueActions)
                        {
                            // Count the number of times this user performed this action on an article with this tag
                            // We can loop through like this since the list is sorted
                            int tagActionCount = 0;
                            for (int i = lastFoundIndex; i < totalUserActions; i++)
                            {
                                if (userActionTags[i].UserID == user.UserID && userActionTags[i].Action == action && userActionTags[i].TagName == tag.Name)
                                {
                                    lastFoundIndex = i;
                                    tagActionCount = userActionTags[i].Count;
                                    break;
                                }
                            }

                            w.Write("," + tagActionCount);
                        }
                    }
                    w.WriteLine();
                }
            }
        }
    }
}
