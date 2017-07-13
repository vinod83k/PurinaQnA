using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    public class CancelDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(MessageUtility.GetCustomerCareMessage(context.MakeMessage()));
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result) {
            context.Done<object>(null);
        }
    }
}