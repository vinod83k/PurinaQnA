using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using QnAMakerDialog;
using System.Threading.Tasks;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    [QnAMakerService("7ece4e3bc1aa4779b72f5fc244696112", "9ab9029c-7b97-4271-bd2c-4dab8c90eacd")]
    public class BasicQnAMakerDialog : QnAMakerDialog<object>
    {
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync($"Sorry, I couldn't find an answer for '{originalQueryText}'.");
            context.Wait(MessageReceived);
        }

        [QnAMakerResponseHandler(50)]
        public async Task LowScoreHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            await context.PostAsync($"I found an answer that might help...{result.Answer}.");
            context.Wait(MessageReceived);
        }

        public override async Task DefaultMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            var messageActivity = ProcessResultAndCreateMessageActivity(context, ref result);
            messageActivity.Text = $"I found an answer that might help...{result.Answer}.";

            await context.PostAsync(messageActivity);

            context.Wait(MessageReceived);
        }
    }
}