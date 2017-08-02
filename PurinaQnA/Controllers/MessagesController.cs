using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Bot.Builder.FormFlow;
using System.Diagnostics;
using System.Web.Http.Description;
using PurinaQnA.Dialog;

namespace PurinaQnA
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {

            //#region Set CurrentBaseURL
            //// Get the base URL that this service is running at
            //// This is used to show images
            //string CurrentBaseURL =
            //        this.Url.Request.RequestUri.AbsoluteUri.Replace(@"api/messages", "");
            //// Create an instance of BotData to store data
            //BotData objBotData = new BotData();
            //// Instantiate a StateClient to save BotData            
            //StateClient stateClient = activity.GetStateClient();
            //// Use stateClient to get current userData
            //BotData userData = await stateClient.BotState.GetUserDataAsync(
            //    activity.ChannelId, activity.From.Id);

            //// Update userData by setting CurrentBaseURL and Recipient
            //userData.SetProperty<string>("CurrentBaseURL", CurrentBaseURL);
            //// Save changes to userData
            //await stateClient.BotState.SetUserDataAsync(
            //    activity.ChannelId, activity.From.Id, userData);
            //#endregion


            if (activity != null)
            {
                IConversationUpdateActivity update = activity;
                var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());

                try
                {
                    activity.Recipient.Name = Resources.ChatBot.BotName;

                    // one of these will have an interface and process it
                    switch (activity.GetActivityType())
                    {
                        case ActivityTypes.Message:

                            var messageHandler = Task.Run(() => Conversation.SendAsync(activity, () => new ExceptionHandlerDialog<object>(new RootDialog(), displayException: true)));
                            messageHandler.Wait();
                            var replyRunning = activity.CreateReply();
                            if (messageHandler.Status == TaskStatus.Running) {
                            }
                            if (messageHandler.IsCompleted) {
                                await client.Conversations.ReplyToActivityAsync(replyRunning);
                                //var reply = activity.CreateReply();
                                //reply.Text = "Task Completed";
                                //await client.Conversations.ReplyToActivityAsync(reply);
                            }

                            //await Conversation.SendAsync(activity, () => new ExceptionHandlerDialog<object>(new RootDialog(), displayException: true));
                            break;

                        case ActivityTypes.ConversationUpdate:
                            if (activity.MembersAdded.Any(x => x.Id == activity.Recipient.Id))
                            {
                                ThumbnailCard plCard = new ThumbnailCard();
                                List<CardAction> cardButtons = new List<CardAction>();

                                cardButtons.Add(new CardAction { Title = "FAQ", Type = ActionTypes.ImBack, Value = "FAQ" });
                                cardButtons.Add(new CardAction { Title = "Retailer Finder", Type = ActionTypes.ImBack, Value = "Retailer Finder" });

                                plCard.Images = new List<CardImage> { new CardImage { Url = string.Format(@"{0}/{1}", this.Url.Request.RequestUri.AbsoluteUri.Replace(@"api/messages", ""), "Images/PurinaChatBot.png") } };
                                plCard.Text = Resources.ChatBot.WelcomeMessage;
                                plCard.Buttons = cardButtons;

                                var reply = activity.CreateReply();
                                reply.Attachments.Add(plCard.ToAttachment());

                                await client.Conversations.ReplyToActivityAsync(reply);
                            }
                            break;
                        case ActivityTypes.ContactRelationUpdate:
                        case ActivityTypes.Typing:
                            var typingReply = activity.CreateReply();
                            typingReply.Text = "Typing...";
                            await client.Conversations.ReplyToActivityAsync(typingReply);
                            break;
                        case ActivityTypes.DeleteUserData:
                        default:
                            Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                            break;
                    }

                }
                catch (Exception ex)
                {
                    //var exReply = activity.CreateReply(ex.Message);
                    //await client.Conversations.ReplyToActivityAsync(exReply);
                }
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