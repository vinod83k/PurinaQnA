using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    public class RetailerFinderDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Please enter zipcode/city/state name to find retailer.");
            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            
            context.Call(Chain.From(() => FormDialog.FromForm(RetailerFinder.BuildForm)), ResumeAfterFormDialog);

            context.Wait(MessageReceivedAsync);
        }

        private async Task AfterMenuSelection(IDialogContext context, IAwaitable<string> result)
        {
            var optionSelected = await result;
            //await context.Forward(new BasicQnAMakerDialog(), ResumeAfterOptionDialog, context.Activity.AsMessageActivity());
        }
    }
}