namespace PurinaQnA.Actions
{
    using Dialog;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("NotHappy")]
    public class NotHappyAction : BaseLuisAction
    {
        public override async Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            await context.PostAsync(Resources.ChatBot.NotHappyMessage);
            await context.Forward(new ContactUsDialog(), ResumeAfterContactUsDialog, null, CancellationToken.None);

            return Task.FromResult((object)"");
        }

        private async Task ResumeAfterContactUsDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var isMsgHandled = await result;
            if (isMsgHandled) {
                context.Done(true);
            }
            else
            {
                await context.Forward(new RootDialog(), ResumeAfterRootDialog, context.Activity.AsMessageActivity(), CancellationToken.None);
                context.Done(true);
            }
        }

        private async Task ResumeAfterRootDialog(IDialogContext context, IAwaitable<object> result) {

        }
    }
}