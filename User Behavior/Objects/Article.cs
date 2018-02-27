using System;
using System.Collections.Generic;
using System.Linq;

namespace UserBehavior.Objects
{
    [Serializable]
    public class Article
    {
        public int ArticleID { get; set; }

        public string Name { get; set; }

        public List<Tag> Tags { get; set; }

        public Article(int id, string name, List<Tag> tags)
        {
            ArticleID = id;
            Name = name;
            Tags = tags;
        }

        public override string ToString()
        {
            return ArticleID + "," + Name + "," + Tags.Select(x => x.Name).Aggregate((c, n) => c + "," + n);
        }
    }
}
