using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using QnAMakerDialog;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

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
            var message = context.MakeMessage();
            //message.Text = $"I found an answer that might help...{result.Answer}.";
            message.Text = "did you mean...";
            message.TextFormat = TextFormatTypes.Plain;
            message.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>
                {
                    new CardAction{ Title = "TopQ", Type=ActionTypes.ImBack, Value="TopQText" },
                    new CardAction{ Title = "Advisor", Type=ActionTypes.ImBack, Value="AdvisorText" },
                    new CardAction{ Title = "Reading", Type=ActionTypes.ImBack, Value="ReadingText" },
                    new CardAction{ Title = result.Answer.Substring(0, 20) , Type=ActionTypes.ImBack, Value= result.Answer.Substring(0, 20) }
                }
            };
            await context.PostAsync(message);

            context.Wait(MessageReceived);

            //await context.PostAsync($"I found an answer that might help...{result.Answer}.");
            //context.Wait(MessageReceived);
        }

        //public override async Task DefaultMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        //{
        //    //var messageActivity = ProcessResultAndCreateMessageActivity(context, ref result);
        //    //messageActivity.Text = $"I found an answer that might help...{result.Answer}.";

        //    //await context.PostAsync(messageActivity);

        //    //context.Wait(MessageReceived);
        //}
    }
}