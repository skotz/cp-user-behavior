using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserBehavior
{
    class UserBehaviorWebService
    {
        private string azureEndpoint;
        private string azureApiKey;

        public UserBehaviorWebService(string endpoint, string apikey)
        {
            azureEndpoint = endpoint;
            azureApiKey = apikey;
        }

        public List<int> GetSuggestedArticles(int userid)
        {
            return new List<int>();
        }
    }
}
