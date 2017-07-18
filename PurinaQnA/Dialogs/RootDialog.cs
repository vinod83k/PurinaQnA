using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using PurinaQnA.Utils;
using PurinaQnA.Enumerations;
using System.Linq;
using AdaptiveCards;
using Microsoft.Bot.Builder.FormFlow;
using System.Threading;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog
    {

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            try
            {
                if (message.Text.ToLower().Contains("retailer finder"))
                {
                    try
                    {
                        await context.Forward(Chain.From(() => FormDialog.FromForm(RetailerFinder.BuildForm)), ResumeAfterFormDialog, message, CancellationToken.None);
                    }
                    catch (Exception)
                    {
                        await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity.AsMessageActivity(), $"We could not find retailers for your location. Please try with another Zip/City/State by selecting 'Retailer Finder'."));
                        context.Wait(MessageReceivedAsync);
                    }
                }
                else if (message.Text.ToLower().Contains("faq"))
                {
                    await context.PostAsync(MessageUtility.GetSimpleTextMessage(context.MakeMessage(), "Please enter your question"));
                    await context.Forward(new BasicQnAMakerDialog(), ResumeAfterQnaMakerDialog, message, CancellationToken.None);
                }
                else
                {
                    await context.Forward(new CommonResponsesDialog(), ResumeAfterCommonResponseDialog, message, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                //await context.PostAsync(ex.Message);
                //context.Wait(MessageReceivedAsync);
            }
        }


        private async Task ResumeAfterCommonResponseDialog(IDialogContext context, IAwaitable<CommonResponseResult> result)
        {
            var messageHandled = await result as CommonResponseResult; 
            if (messageHandled!= null && messageHandled.IsMessageHandled && messageHandled.Message.ToLower().Contains("animals"))
            {
                context.Call(new BasicQnAMakerDialog(), ResumeAfterQnaMakerDialog);
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity, "Are you looking for"));
            }
        }

        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<object> result) {
            RetailerFinder msgResult = null;
            try
            {
                msgResult = (RetailerFinder)await result;
            }
            catch (Exception)
            {
            }

            if (msgResult != null && msgResult.ResultCount > 0) {
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity.AsMessageActivity(), "Hope you find your retailers. Please select if you are looking for FAQ"));
            }
            else
            {
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity.AsMessageActivity(), $"We could not find retailers for your location. Please try with another Zip/City/State by selecting 'Retailer Finder'."));
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterQnaMakerDialog(IDialogContext context, IAwaitable<object> result)
        {
            var messageResult = await result;
            if (messageResult.ToString().ToLower().Contains("nomatch"))
            {
                var messageActivity = context.MakeMessage();
                await context.PostAsync(MessageUtility.GetSimpleTextMessage(messageActivity, "We could not find answer to your question"));

                await context.PostAsync(GetMessage(context, ""));
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity, "Or you can look for more options below"));
                context.Done("null");
            }
            else if (messageResult.ToString().ToLower().Contains("retailer")) {
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity, "Looking for"));
                context.Done("null");
            }
            else
            {
                context.Wait(MessageReceivedAsync);
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
            messageActivity.From.Name = Resources.ChatBot.BotName;
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
                Title = "What's Best For Your Animals"                
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);
            return messageActivity;
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            //This means  MessageRecievedAsync function of this dialog (PromptButtonsDialog) will receive users' messeges
            context.Wait(MessageReceivedAsync);
        }

    }
}