namespace PurinaQnA.Actions
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("GoodBye")]
    public class GoodByeAction : BaseLuisAction
    {
        public override Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            return Task.FromResult((object)"Bye. Looking forward to our next awesome conversation already.");
        }
    }
}