using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserBehavior.Objects;

namespace UserBehavior.Abstractions
{
    public interface IRater
    {
        double GetRating(List<UserAction> actions);
    }
}
