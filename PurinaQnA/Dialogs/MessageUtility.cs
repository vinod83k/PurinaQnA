using AdaptiveCards;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace PurinaQnA.Dialogs
{
    public static class MessageUtility
    {
        public static Activity GetWelcomeOptionsMessage(Activity activity, string textMsg)
        {
            var reply = activity.CreateReply();
            reply.From.Name = Resources.ChatBot.BotName;
            var replyProp = reply.From.Properties;
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

            cardButtons.Add(new CardAction { Title = "FAQ", Type = ActionTypes.ImBack, Value = "FAQ" });
            cardButtons.Add(new CardAction { Title = "Retailer Finder", Type = ActionTypes.ImBack, Value = "Retailer Finder" });

            plCard.Title = textMsg;
            plCard.Buttons = cardButtons;

            Attachment attachmentpl = plCard.ToAttachment();

            messageActivity.Attachments.Add(attachmentpl);
            return messageActivity;
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