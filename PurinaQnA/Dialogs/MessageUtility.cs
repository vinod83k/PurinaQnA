using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurinaQnA.Dialogs
{
    public static class MessageUtility
    {

        public static Activity GetWelcomeMessage(IDialogContext context, Activity activity, string strMsg)
        {
            string imageUrl;
            context.UserData.TryGetValue<string>("CurrentBaseURL", out imageUrl);
            return GetWelcomeOptionsMessage(activity, strMsg, imageUrl);
        }


        public static Activity GetWelcomeOptionsMessage(Activity activity, string textMsg, string imageUrl = "")
        {
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            ThumbnailCard plCard = new ThumbnailCard();
            List<CardAction> cardButtons = new List<CardAction>();

            cardButtons.Add(new CardAction { Title="FAQ", Type = ActionTypes.ImBack, Value = "FAQ" });
            cardButtons.Add(new CardAction { Title = "Retailer Finder", Type = ActionTypes.ImBack, Value = "Retailer Finder" });

            plCard.Subtitle = textMsg;
            plCard.Buttons = cardButtons;

            Attachment attachmentpl = plCard.ToAttachment();

            reply.Attachments.Add(attachmentpl);
            return reply;
        }

        public static IMessageActivity GetWelcomeMessageWithOptions(IMessageActivity messageActivity, string textMsg)
        {
            //var reply = activity.CreateReply();
            messageActivity.Attachments = new List<Attachment>();
            messageActivity.From.Name = Resources.ChatBot.BotName;
            ThumbnailCard plCard = new ThumbnailCard();
            List<CardAction> cardButtons = new List<CardAction>();

            cardButtons.Add(new CardAction { Title = Resources.ChatBot.TitleFaq, Type = ActionTypes.ImBack, Value = Resources.ChatBot.TitleFaq });
            cardButtons.Add(new CardAction { Title = Resources.ChatBot.TitleRetailerFinder, Type = ActionTypes.ImBack, Value = Resources.ChatBot.TitleRetailerFinder });

            plCard.Title = textMsg;
            plCard.Buttons = cardButtons;

            Attachment attachmentpl = plCard.ToAttachment();

            messageActivity.Attachments.Add(attachmentpl);
            return messageActivity;
        }

        public static void GetThumbnailCardMessage(IMessageActivity messageActivity, string textMsg, IList<CardAction> buttons)
        {
            //var reply = activity.CreateReply();
            messageActivity.Attachments = new List<Attachment>();
            ThumbnailCard plCard = new ThumbnailCard();

            //plCard.Title = textMsg;
            plCard.Text = textMsg;
            plCard.Buttons = buttons;


            Attachment attachmentpl = plCard.ToAttachment();

            messageActivity.Attachments.Add(attachmentpl);
            //return messageActivity;
        }

        public static IMessageActivity GetSimpleTextMessage(IMessageActivity messageActivity, string textMsg)
        {
            messageActivity.Attachments = new List<Attachment>();
            messageActivity.From.Name = Resources.ChatBot.BotName;
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = textMsg,
                Size = TextSize.Large,
                Weight = TextWeight.Normal,
                Wrap = true
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);

            return messageActivity;
        }

        public static IMessageActivity DisplayContactUsAdaptiveCard(IMessageActivity messageActivity, string textMsg)
        {
            messageActivity.Attachments = new List<Attachment>();

            AdaptiveCard card = new AdaptiveCard();
            var inputName = new TextInput {
                Id = "UserName",
                Placeholder = "Last, First",
                IsRequired = true,
                IsMultiline = false                
            };

            card.Body.Add(new ColumnSet {
                Columns = new List<Column> {
                    new Column {
                        Items = new List<CardElement> {
                            new TextBlock { Text = "Your Name", Wrap = true },
                            inputName
                        }
                    }              
                }                
            });

            var submitActionData = JsonConvert.SerializeObject(card.Body);

            card.Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Search",
                        Speak = "<s>Search</s>",
                        DataJson = "{ \"Type\": \"HotelSearch\" }"
                    }
                };

        //    card.Actions.Add(new SubmitAction {
        //        DataJson = submitActionData.ToString(),
        //        //Data = submitActionData.ToString(),
        //        Title = "Submit"
        //});

            //card.Actions.Add(new HttpAction {
            //    Url = "http://localhost:3979/api/ContactUs",
            //    Title = "Submit",
            //    Method = "Post",
            //    Body = "{ \"username\": \"UserName\" }"
            //});

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);

            return messageActivity;
        }

        public static IMessageActivity GetThanksMessage(IMessageActivity messageActivity, string textMsg)
        {
            messageActivity.Attachments = new List<Attachment>();
            messageActivity.From.Name = Resources.ChatBot.BotName;
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = "Glad, you find it useful. If you are looking for something else, you can always navigate to the following link:",
                Size = TextSize.Large,
                Weight = TextWeight.Normal,
                Wrap = true
            });

            // Add buttons to the card.
            card.Actions.Add(new OpenUrlAction()
            {
                Url = "http://tst2.purinamills.com/",
                Title = "Purina",
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);

            return messageActivity;
        }

        public static IMessageActivity GetCustomerCareMessage(IMessageActivity messageActivity)
        {
            
            messageActivity.Attachments = new List<Attachment>();
            messageActivity.From.Name = Resources.ChatBot.BotName;
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = "We apologize if we could not serve you well. If you have any questions, one of our nutrition experts may be able to help",
                Size = TextSize.Large,
                Weight = TextWeight.Normal,
                Wrap = true
            });

            // Add buttons to the card.
            card.Actions.Add(new OpenUrlAction()
            {
                Url = "https://www.purinamills.com/ask-an-expert",
                Title = "Ask an Expert",
            });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            messageActivity.Attachments.Add(attachment);

            return messageActivity;
        }
    }
}