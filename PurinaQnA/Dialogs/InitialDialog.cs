using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using PurinaQnA.Utils;
using PurinaQnA.Enumerations;
using System.Linq;
using AdaptiveCards;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    public class InitialDialog : IDialog
    {
        private class InitialDialogMessage {
            public bool IsMsgHandled { get; set; }
            public IMessageActivity MessageActivity { get; set; }
        }

        public async Task StartAsync(IDialogContext context)
        {
            await HandleInitialDialog(context);
            context.Wait(MessageRecievedAsync);
        }

        private async Task<InitialDialogMessage> HandleInitialDialog(IDialogContext context) {
            var queryText = context.Activity.AsMessageActivity().Text;
            var categoryFound = Enumeration.GetAll<Categories>().FirstOrDefault(x => x.DisplayName == queryText);
            IMessageActivity messageActivity; bool isMsgHandled = false;
            if (categoryFound != null && !string.IsNullOrEmpty(categoryFound.FeedUrl))
            {
                messageActivity = GetFeedMessage(context, queryText, categoryFound);
                isMsgHandled = true;
            }
            else
            {
                messageActivity = GetMessage(context, queryText);
            }

            return new InitialDialogMessage { IsMsgHandled = isMsgHandled, MessageActivity = messageActivity };
        }

        public virtual async Task MessageRecievedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var option = await result;
            var initialDialogMsg = await HandleInitialDialog(context);
            if (initialDialogMsg.IsMsgHandled) {
                await context.PostAsync(initialDialogMsg.MessageActivity);
                context.Wait(MessageRecievedAsync);
            }
            else
            {
                await context.Forward(new BasicQnAMakerDialog(), ResumeAfterOptionDialog, context.Activity.AsMessageActivity());
                context.Wait(MessageRecievedAsync);
            }
        }

        private static IMessageActivity GetFeedMessage(IDialogContext context, string queryText, Categories categoryFound)
        {
            var messageActivity = context.MakeMessage();
            messageActivity.Attachments = new List<Attachment>();
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = $"Are you looking for '{queryText}' feed ?",
                Size = TextSize.Medium,
                Weight = TextWeight.Normal,
                Wrap = true
            });

            card.Body.Add(new TextBlock()
            {
                Text = "You can find more information here",
                Size = TextSize.Large,
                Weight = TextWeight.Bolder,
                Wrap = true
            });

            // Add buttons to the card.
            card.Actions.Add(new OpenUrlAction()
            {
                Url = categoryFound.FeedUrl,
                Title = $"{queryText} feed",
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);
            return messageActivity;
        }
        private static IMessageActivity GetMessage(IDialogContext context, string queryText)
        {
            var messageActivity = context.MakeMessage();
            messageActivity.Attachments = new List<Attachment>();
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = "You can find more information here",
                Size = TextSize.Large,
                Weight = TextWeight.Bolder,
                Wrap = true
            });

            // Add buttons to the card.
            card.Actions.Add(new OpenUrlAction()
            {
                Url = "http://tst2.purinamills.com/animal-nutrition-information",
                Title = "Learn More About What's Best For Your Animals"
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);
            return messageActivity;
        }

        private async Task AfterCommonResponseHandled(IDialogContext context, IAwaitable<bool> result)
        {
            var messageHandled = await result;

            if (!messageHandled)
            {
                await context.Forward(new BasicQnAMakerDialog(), ResumeAfterOptionDialog, context.Activity.AsMessageActivity());
            }

            context.Wait(MessageRecievedAsync);
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            //This means  MessageRecievedAsync function of this dialog (PromptButtonsDialog) will receive users' messeges
            context.Wait(MessageRecievedAsync);
        }

    }
}