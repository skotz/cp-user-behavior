using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Abstractions;
using UserBehavior.Objects;

namespace UserBehavior.Parsers
{
    public class UserBehaviorTransformer
    {
        private UserBehaviorDatabase db;

        public UserBehaviorTransformer(UserBehaviorDatabase database)
        {
            db = database;
        }
        
        /// <summary>
        /// Get a list of all users and their ratings on every article
        /// </summary>
        public UserArticleRatingsTable GetUserArticleRatingsTable(IRater rater)
        {
            UserArticleRatingsTable table = new UserArticleRatingsTable();
            
            table.UserIndexToID = db.Users.OrderBy(x => x.UserID).Select(x => x.UserID).Distinct().ToList();
            table.ArticleIndexToID = db.Articles.OrderBy(x => x.ArticleID).Select(x => x.ArticleID).Distinct().ToList();
            table.NumberOfTags = db.Tags.Count;

            foreach (int userId in table.UserIndexToID)
            {
                table.UserArticleRatings.Add(new UserArticleRatings(userId, table.ArticleIndexToID.Count));
            }

            var userArticleRatingGroup = db.UserActions
                .GroupBy(x => new { x.UserID, x.ArticleID })
                .Select(g => new { g.Key.UserID, g.Key.ArticleID, Rating = rater.GetRating(g.ToList()) })
                .ToList();

            foreach (var userAction in userArticleRatingGroup)
            {
                int userIndex = table.UserIndexToID.IndexOf(userAction.UserID);
                int articleIndex = table.ArticleIndexToID.IndexOf(userAction.ArticleID);

                table.UserArticleRatings[userIndex].ArticleRatings[articleIndex] = userAction.Rating;
            }

            return table;
        }

        /// <summary>
        /// Get a table of all articles as rows and all tags as columns
        /// </summary>
        public List<ArticleTagCounts> GetArticleTagCounts()
        {
            List<ArticleTagCounts> articleTags = new List<ArticleTagCounts>();

            foreach (Article article in db.Articles)
            {
                ArticleTagCounts articleTag = new ArticleTagCounts(article.ArticleID, db.Tags.Count);

                for (int tag = 0; tag < db.Tags.Count; tag++)
                {
                    articleTag.TagCounts[tag] = article.Tags.Any(x => x.Name == db.Tags[tag].Name) ? 1.0 : 0.0;
                }

                articleTags.Add(articleTag);
            }

            return articleTags;
        }

        /// <summary>
        /// Get a list of all users and the number of times they viewed articles with a specific tag
        /// </summary>
        [Obsolete]
        public List<UserActionTag> GetUserTags()
        {
            List<UserActionTag> userData = new List<UserActionTag>();
            List<ArticleTag> articleTags = db.GetArticleTagLinkingTable();

            int numFeatures = db.Tags.Count;

            // Create a dataset that links every user action to a list of tags associated with the article that action was for, then 
            // group them by user, action, and tag so we can get a count of the number of times each user performed a action on an article with a specific tag
            var userActionTags = db.UserActions
                .Join(articleTags, u => u.ArticleID, t => t.ArticleID, (u, t) => new { u.UserID, t.TagName })
                .GroupBy(x => new { x.UserID, x.TagName })
                .Select(g => new { g.Key.UserID, g.Key.TagName, Count = g.Count() })
                .OrderBy(x => x.UserID).ThenBy(x => x.TagName)
                .ToList();

            int totalUserActions = userActionTags.Count();
            int lastFoundIndex = 0;

            // Get action-tag data
            foreach (User user in db.Users)
            {
                int dataCol = 0;
                UserActionTag uat = new UserActionTag(user.UserID, numFeatures);

                foreach (Tag tag in db.Tags)
                {
                    // Count the number of times this user interacted with an article with this tag
                    // We can loop through like this since the list is sorted
                    int tagActionCount = 0;
                    for (int i = lastFoundIndex; i < totalUserActions; i++)
                    {
                        if (userActionTags[i].UserID == user.UserID && userActionTags[i].TagName == tag.Name)
                        {
                            lastFoundIndex = i;
                            tagActionCount = userActionTags[i].Count;
                            break;
                        }
                    }

                    uat.ActionTagData[dataCol++] = tagActionCount;
                }

                // Normalize data to values between 0 and 5
                double upperLimit = 5.0;
                double max = uat.ActionTagData.Max();
                if (max > 0)
                {
                    for (int i = 0; i < uat.ActionTagData.Length; i++)
                    {
                        uat.ActionTagData[i] = (uat.ActionTagData[i] / max) * upperLimit;
                    }
                }

                userData.Add(uat);
            }

            return userData;
        }
    }
}
