namespace PurinaQnA.Actions
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("FAQ")]
    //public class FaqAction : BaseLuisContextualAction<AnimalsAction>
    public class FaqAction : BaseLuisAction
    {
        public override async Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            //await context.PostAsync(Resources.ChatBot.FaqMessage);
            return Task.FromResult((object)Resources.ChatBot.FaqMessage);
        }
    }
}