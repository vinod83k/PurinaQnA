namespace PurinaQnA.Actions
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("Greetings")]
    public class GreetingsAction : BaseLuisAction
    {
        public override Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            return Task.FromResult((object)Resources.ChatBot.GreetingsActionMessage);
        }
    }
}