using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using QnAMakerDialog;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AdaptiveCards;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    [QnAMakerService("7ece4e3bc1aa4779b72f5fc244696112", "9ab9029c-7b97-4271-bd2c-4dab8c90eacd")]
    public class BasicQnAMakerDialog : QnAMakerDialog<object>
    {
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            if (originalQueryText.ToLower().Contains("faq") || originalQueryText.ToLower().Contains("question")) {
                context.Wait(MessageReceived);
            }
            else
            {
                context.Done("NoMatch");
            }
        }

        [QnAMakerResponseHandler(50)]
        public async Task LowScoreHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            var messageActivity = ProcessResultAndCreateMessageActivity(context, ref result);
            messageActivity.Attachments = new List<Attachment>();
            AdaptiveCard card = new AdaptiveCard();

            // Add text to the card.
            card.Body.Add(new TextBlock()
            {
                Text = "I found an answer that might help...",
                Size = TextSize.Large,
                Weight = TextWeight.Normal,
                Wrap = true
            });

            card.Body.Add(new TextBlock()
            {
                Text = result.Answer,
                Wrap = true
            });

            card.Body.Add(new TextBlock()
            {
                Text = "If you are looking for something else, you can always navigate to the following link:",
                Size = TextSize.Large,
                Weight = TextWeight.Normal,
                Wrap = true
            });

            // Add buttons to the card.
            card.Actions.Add(new OpenUrlAction()
            {
                Url = "http://tst2.purinamills.com/",
                Title = "Purina FAQ",
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);

            await context.PostAsync(messageActivity);
            context.Wait(MessageReceived);
        }

        public override async Task DefaultMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            var messageActivity = ProcessResultAndCreateMessageActivity(context, ref result);
            messageActivity.Attachments = new List<Attachment>();
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = result.Answer,
                Wrap = true
            });

            card.Body.Add(new TextBlock()
            {
                Text = "You can always find more information at below link...",
                Size = TextSize.Medium,
                Weight = TextWeight.Normal,
                Wrap = true
            });

            // Add buttons to the card.
            card.Actions.Add(new OpenUrlAction()
            {
                Url = "http://tst2.purinamills.com/",
                Title = "Purina FAQ",
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);

            await context.PostAsync(messageActivity);
            context.Wait(MessageReceived);

        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            //context.Wait(MessageReceived);
        }


    }
}