using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System;
using System.Linq;
using PurinaQnA.Enumerations;
using PurinaQnA.Utils;
using PurinaQnA.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Diagnostics;
using System.Web.Http.Description;

namespace PurinaQnA
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        internal static IDialog<RetailerFinder> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(RetailerFinder.BuildForm));
        }

        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {
                // one of these will have an interface and process it
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, () => new PurinaDialogs.RootDialog());
                        //await Conversation.SendAsync(activity, () => new Dialog.RootDialog());
                        break;

                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        if (activity.MembersAdded.Any(x => x.Id == activity.Recipient.Id))
                        {
                            IConversationUpdateActivity update = activity;
                            var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                            var reply = MessageUtility.GetWelcomeOptionsMessage(activity, "Welcome! I am a ChatBot and can help you with what's best for you animals and Retailer finder. Please click on the below options");
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }
            else
            {
                
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }


        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        //{
        //    if (activity.Type == ActivityTypes.Message)
        //    {

        //        //await Conversation.SendAsync(activity, () => new Dialogs.InitialDialog());
        //        await Conversation.SendAsync(activity, () => new Dialogs.RetailerFinderDialog());
        //    }
        //    else
        //    {
        //        if (activity.MembersAdded.Any(x => x.Id == activity.Recipient.Id))
        //        {
        //            IConversationUpdateActivity update = activity;
        //            var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
        //            var reply = activity.CreateReply();
        //            reply.Text = $"Welcome! I am a ChatBot for Purina FAQ. \n\n Looking what's best for your Animals. \n\n Browse topics by...";
        //            var cardActions = new List<CardAction>();
        //            foreach (var category in Enumeration.GetAll<Categories>().Where(x => x.DisplayName != Categories.All.DisplayName))
        //            {
        //                cardActions.Add(new CardAction
        //                {
        //                    Title = category.DisplayName,
        //                    Type = ActionTypes.ImBack,
        //                    Value = category.DisplayName
        //                });
        //            }

        //            reply.Attachments.Add(new ThumbnailCard {
        //                Buttons = cardActions,
        //            }.ToAttachment());
                    
        //            await client.Conversations.ReplyToActivityAsync(reply);
        //        }
        //        HandleSystemMessage(activity);
        //    }

        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}

        //private Activity HandleSystemMessage(Activity message)
        //{
        //    if (message.Type == ActivityTypes.DeleteUserData)
        //    {
        //        // Implement user deletion here
        //        // If we handle user deletion, return a real message
        //    }
        //    else if (message.Type == ActivityTypes.ConversationUpdate)
        //    {
        //        // Handle conversation state changes, like members being added and removed
        //        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        //        // Not available in all channels
        //    }
        //    else if (message.Type == ActivityTypes.ContactRelationUpdate)
        //    {
        //        // Handle add/remove from contact lists
        //        // Activity.From + Activity.Action represent what happened
        //    }
        //    else if (message.Type == ActivityTypes.Typing)
        //    {
        //        // Handle knowing tha the user is typing
        //        var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
        //        var reply = message.CreateReply();
        //        reply.Text = $"User typing...";
        //        client.Conversations.ReplyToActivityAsync(reply);
        //    }
        //    else if (message.Type == ActivityTypes.Ping)
        //    {
        //    }

        //    return null;
        //}
    }
}