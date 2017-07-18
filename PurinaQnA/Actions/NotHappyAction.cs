using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurinaQnA.Actions
{
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("NotHappy")]
    public class NotHappyAction : BaseLuisAction
    {
        public override Task<object> FulfillAsync()
        {
            return Task.FromResult((object)"If I am not able to answer your questions, maybe our Customer Care will be more helpful.");
        }
    }
}