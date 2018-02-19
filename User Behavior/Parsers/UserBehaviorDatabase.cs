using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class UserBehaviorDatabase
    {
        public List<Tag> Tags { get; set; }

        public List<Article> Articles { get; set; }

        public List<User> Users { get; set; }

        public List<UserAction> UserActions { get; set; }

        public UserBehaviorDatabase()
        {
            Tags = new List<Tag>();
            Articles = new List<Article>();
            Users = new List<User>();
            UserActions = new List<UserAction>();
        }

        public List<ArticleTag> GetArticleTagLinkingTable()
        {
            List<ArticleTag> articleTags = new List<ArticleTag>();

            foreach (Article article in Articles)
            {
                foreach (Tag tag in article.Tags)
                {
                    articleTags.Add(new ArticleTag(article.ArticleID, tag.Name));
                }
            }

            return articleTags;
        }
    }
}
