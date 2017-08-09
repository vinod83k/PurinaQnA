namespace PurinaQnA.Actions
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("AboutLoren")]
    public class AboutMeAction : BaseLuisAction
    {
        public override async Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            return Task.FromResult((object)Resources.ChatBot.FaqMessage);
        }
    }
}