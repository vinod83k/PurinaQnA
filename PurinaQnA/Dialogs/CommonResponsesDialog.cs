using AdaptiveCards;
using BestMatchDialog;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurinaQnA.Dialogs
{
    public class CommonResponseResult
    {
        public bool IsMessageHandled { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class CommonResponsesDialog : BestMatchDialog<CommonResponseResult>
    {
        BasicQnAMakerDialog qnaMakerDialog;
        public CommonResponsesDialog() {
            qnaMakerDialog = new BasicQnAMakerDialog();
        }

        public override async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceived);
        }

        [BestMatch(new string[] { "Hi", "Hi There", "Hello there", "Hey", "Hello",
            "Hey there", "Greetings", "Good morning", "Good afternoon", "Good evening", "Good day" },
            threshold: 0.5, ignoreCase: true, ignoreNonAlphaNumericCharacters: false)]
        public async Task HandleGreeting(IDialogContext context, string messageText)
        {
            await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity, Resources.ChatBot.GreetingsMessage));
            context.Wait(MessageReceived);
        }

        [BestMatch("bye|bye bye|got to go|see you later|laters|adios", listDelimiter: '|')]
        public async Task HandleGoodbye(IDialogContext context, string messageText)
        {
            await context.PostAsync(MessageUtility.GetSimpleTextMessage(context.MakeMessage(), "Bye. Looking forward to our next awesome conversation already."));
            context.Wait(MessageReceived);
        }

        [BestMatch("thanks", listDelimiter: '|')]
        public async Task HandleThanks(IDialogContext context, string messageText)
        {
            await context.PostAsync(MessageUtility.GetThanksMessage(context.MakeMessage(), ""));
            context.Wait(MessageReceived);
        }

        [BestMatch("no|not|nothing", listDelimiter: '|')]
        public async Task HandleNo(IDialogContext context, string messageText)
        {
            var messageActivity = context.MakeMessage();
            messageActivity.Attachments = new List<Attachment>();
            AdaptiveCard card = new AdaptiveCard();

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
            await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)messageActivity, "Or you can always search for FAQ's by selecting FAQ"));
            context.Wait(MessageReceived);
        }

        public override async Task NoMatchHandler(IDialogContext context, string messageText)
        {
            context.Done(false);
        }
    }
}