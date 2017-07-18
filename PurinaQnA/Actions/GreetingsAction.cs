namespace PurinaQnA.Actions
{
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("Greetings")]
    public class GreetingsAction : BaseLuisAction
    {
        public override Task<object> FulfillAsync()
        {
            return Task.FromResult((object)"Well hello there. What can I do for you today? Please click on below options");
        }
    }
}