namespace PurinaQnA.Actions
{
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("GoodBye")]
    public class GoodByeAction : BaseLuisAction
    {
        public override Task<object> FulfillAsync()
        {
            return Task.FromResult((object)"Bye. Looking forward to our next awesome conversation already.");
        }
    }
}