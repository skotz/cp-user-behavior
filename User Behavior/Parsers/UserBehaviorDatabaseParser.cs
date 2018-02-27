using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Objects;

namespace UserBehavior.Parsers
{
    public class UserBehaviorDatabaseParser
    {
        private const string HeaderTags = "# Tags";
        private const string HeaderArticles = "# Articles";
        private const string HeaderUsers = "# Users";
        private const string HeaderUserActions = "# User actions";

        public UserBehaviorDatabaseParser()
        {

        }

        public UserBehaviorDatabase LoadUserBehaviorDatabase(string file)
        {
            UserBehaviorDatabase db = new UserBehaviorDatabase();
            
            List<string> lines = File.ReadAllLines(file).ToList();

            // Get the indexes of each data section
            int tagsIndex = lines.FindIndex(x => x.StartsWith(HeaderTags));
            int articlesIndex = lines.FindIndex(x => x.StartsWith(HeaderArticles));
            int usersIndex = lines.FindIndex(x => x.StartsWith(HeaderUsers));
            int userActionsIndex = lines.FindIndex(x => x.StartsWith(HeaderUserActions));

            // Parse out the tags
            for (int i = tagsIndex; i < articlesIndex; i++)
            {
                if (!lines[i].Trim().StartsWith("#") && lines[i].Trim().Length > 0)
                {
                    foreach (string s in lines[i].Split(','))
                    {
                        db.Tags.Add(new Tag(s.Trim()));
                    }
                }
            }

            // Parse out the articles
            for (int i = articlesIndex; i < usersIndex; i++)
            {
                if (!lines[i].Trim().StartsWith("#") && lines[i].Trim().Length > 0)
                {
                    string[] cols = lines[i].Split(',');
                    List<Tag> tags = new List<Tag>();

                    for (int c = 2; c < cols.Length; c++)
                    {
                        tags.Add(new Tag(cols[c].Trim()));
                    }

                    db.Articles.Add(new Article(int.Parse(cols[0].Trim()), cols[1].Trim(), tags));
                }
            }
            
            // Parse out the users
            for (int i = usersIndex; i < userActionsIndex; i++)
            {
                if (!lines[i].Trim().StartsWith("#") && lines[i].Trim().Length > 0)
                {
                    string[] cols = lines[i].Split(',');
                    db.Users.Add(new User(int.Parse(cols[0].Trim()), cols[1].Trim()));
                }
            }

            // Parse out the user actions
            for (int i = userActionsIndex; i < lines.Count; i++)
            {
                if (!lines[i].Trim().StartsWith("#") && lines[i].Trim().Length > 0)
                {
                    string[] cols = lines[i].Split(',');
                    db.UserActions.Add(new UserAction(int.Parse(cols[0].Trim()), cols[1].Trim(), int.Parse(cols[2].Trim()), cols[3].Trim(), int.Parse(cols[4].Trim()), cols[5].Trim()));
                }
            }
            
            return db;
        }
    }
}
