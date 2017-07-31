using Microsoft.Cognitive.LUIS.ActionBinding;
using System;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using PurinaQnA.Dialog;

namespace PurinaQnA.Actions
{
    [Serializable]
    [LuisActionBinding("ContactUs")]
    public class ContactUsAction : BaseLuisAction
    {
        public override Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            context.Forward(new ContactUsDialog(), ResumeAfterContactUsDialog, null, CancellationToken.None);

            return Task.FromResult((object)"");
        }

        private async Task ResumeAfterContactUsDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var isMsgHandled = await result;
            if (isMsgHandled)
            {
                context.Done(true);
            }
            else
            {
                await context.Forward(new RootDialog(), ResumeAfterRootDialog, context.Activity.AsMessageActivity(), CancellationToken.None);
                context.Done(true);
            }
        }

        private async Task ResumeAfterRootDialog(IDialogContext context, IAwaitable<object> result)
        {

        }

    }
}