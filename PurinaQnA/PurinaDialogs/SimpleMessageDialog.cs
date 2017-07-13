using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace PurinaQnA.PurinaDialogs
{
    [Serializable]
    public class SimpleMessageDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(Dialogs.MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity, "We apologize, as we could not understand your request. Please click on the below options..."));
            context.Wait(MessageReceivedAsync);
        }
    }
}