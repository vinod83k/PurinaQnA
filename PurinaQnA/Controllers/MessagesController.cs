﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PurinaQnA
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.BasicQnAMakerDialog());
            }
            //else if (activity.Type == ActivityTypes.ConversationUpdate)
            //{
            //    //var reply = activity.CreateReply("WELCOME!!!");
            //    //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            //    //await connector.Conversations.ReplyToActivityAsync(reply);

            //    IConversationUpdateActivity update = activity;
            //    var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
            //    if (update.MembersAdded != null && update.MembersAdded.Any())
            //    {
            //        foreach (var newMember in update.MembersAdded)
            //        {
            //            if (newMember.Id != activity.Recipient.Id)
            //            {
            //                var reply = activity.CreateReply();
            //                reply.Text = $"Welcome! Please enter your question...";
            //                await client.Conversations.ReplyToActivityAsync(reply);
            //            }
            //        }
            //    }
            //}
            else
            {
                if (activity.MembersAdded.Any(x => x.Id == activity.Recipient.Id)) {
                    IConversationUpdateActivity update = activity;
                    var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                    var reply = activity.CreateReply();
                    reply.Text = $"Welcome! I am a ChatBot for Purina FAQ. \n\n Please enter your question...";
                    await client.Conversations.ReplyToActivityAsync(reply);

                }
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                //IConversationUpdateActivity update = message;
                //var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                //if (update.MembersAdded != null && update.MembersAdded.Any())
                //{
                //    foreach (var newMember in update.MembersAdded)
                //    {
                //        if (newMember.Id != message.Recipient.Id)
                //        {
                //            var reply = message.CreateReply();
                //            reply.Text = $"Welcome {newMember.Name}!";
                //            client.Conversations.ReplyToActivityAsync(reply);
                //        }
                //    }
                //}
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}