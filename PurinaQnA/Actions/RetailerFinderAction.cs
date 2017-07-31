namespace PurinaQnA.Actions
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Text;
    using Models;
    using System.Threading;
    using Dialog;

    [Serializable]
    [LuisActionBinding("RetailerFinder")]
    public class RetailerFinderAction : BaseLuisAction
    {
        [Required(ErrorMessage = "Please enter ZipCode/City or State name to find Retailers")]
        public string Location { get; set; }

        private const int TakeResultCount = 5;

        public override async Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            // call service api and get the Retailers
            try
            {
                var retailers = await this.GetRetailerFinderApiResponse(this.Location);
                var messageActivity = this.CreateRetailerMessage(context, retailers);

                if (retailers.ResultCount > TakeResultCount)
                {
                    await context.PostAsync(string.Format(Resources.ChatBot.TopNearestRetailersLocation, TakeResultCount));
                }
                await context.PostAsync(messageActivity);

                PromptDialog.Confirm(
                    context,
                    this.AfterRetailersFinderConfirmDialog,
                    Resources.ChatBot.FindMoreRetailers);
            }
            catch (Exception)
            {
                await context.Forward(new RootDialog(), ResumeAfterRootDialog, context.Activity.AsMessageActivity(), CancellationToken.None);
                context.Done(true);
            }

            return Task.FromResult((object)"");
        }

        private async Task ResumeAfterRootDialog(IDialogContext context, IAwaitable<object> result)
        {

        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result) {
            var message = await result;
            if (message != null) {
                this.Location = message.Text;
                await this.FulfillAsync(context);
            }
            else
            {
                await context.PostAsync(Resources.ChatBot.CouldNotUnderstand);
                await context.Forward(new ContactUsDialog(), ResumeAfterContactUsDialog, null, CancellationToken.None);

                context.Done(true);
            }
        }

        private async Task AfterRetailersFinderConfirmDialog(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result == true)
            {
                var validationResults = default(ICollection<ValidationResult>);
                this.IsValid(out validationResults);
                await context.PostAsync(validationResults.ElementAt(0).ErrorMessage);

                context.Wait(MessageReceivedAsync);
            }
            else
            {
                await context.PostAsync(Resources.ChatBot.WhatElseHelp);
                await context.Forward(new ContactUsDialog(), ResumeAfterContactUsDialog, null, CancellationToken.None);
            }
        }

        private async Task ResumeAfterContactUsDialog(IDialogContext context, IAwaitable<bool> result)
        {
            var isMsgHandled = await result;
            if (isMsgHandled)
            {
                context.Done(true);
            }
            else
            {
                await context.Forward(new RootDialog(), ResumeAfterRootDialog, context.Activity.AsMessageActivity(), CancellationToken.None);
                context.Done(true);
            }
        }


        private async Task<RetailerFinderEntity> GetRetailerFinderApiResponse(string location)
        {
            string responseString = string.Empty;

            ////Build the URI
            Uri qnamakerUriBase = new Uri($"https://apis.landolakesinc.com/EntityLocatorAPI/v2/Service1/Entities?instance=3&zip={location}&city={location}&state={location}&radius=60&attribute=0&max=100");

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                responseString = client.DownloadString(qnamakerUriBase);
            }

            //De-serialize the response
            RetailerFinderEntity response;
            try
            {
                response = JsonConvert.DeserializeObject<RetailerFinderEntity>(responseString);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to deserialize Retailer Finder response string.");
            }
        }

        private IMessageActivity CreateRetailerMessage(IDialogContext context, RetailerFinderEntity entity)
        {
            var messageActivity = context.MakeMessage();

            messageActivity.Attachments = new List<Attachment>();

            if (entity != null && entity.EntityList.Any())
            {

                foreach (var entityItem in entity.EntityList.OrderBy(x => x.Distance).Take(TakeResultCount))
                {

                    AdaptiveCard card = new AdaptiveCard();

                    // Add text to the card.
                    card.Body.Add(new TextBlock
                    {
                        Text = $"{entityItem.Name}",
                        Color = TextColor.Dark,
                        Size = TextSize.Normal,
                        Weight = TextWeight.Bolder,
                        Wrap = true
                    });

                    var address = new StringBuilder();
                    if (!string.IsNullOrEmpty(entityItem.Address1))
                    {
                        address.Append(entityItem.Address1);
                    }
                    if (!string.IsNullOrEmpty(entityItem.Address2))
                    {
                        address.Append(", " + entityItem.Address2);
                    }
                    address.Append($", {entityItem.City}, {entityItem.StateProvince}");

                    if (!string.IsNullOrEmpty(entityItem.PhoneNumber))
                    {
                        address.Append("\n\n" + entityItem.PhoneNumber);
                    }

                    card.Body.Add(new TextBlock
                    {
                        Text = address.ToString(),
                        Size = TextSize.Normal,
                        Wrap = true
                    });


                    // Add buttons to the card.
                    card.Actions.Add(new OpenUrlAction
                    {
                        Url = $"http://maps.google.com/maps/?daddr={entityItem.Address1}, {entityItem.City}, {entityItem.StateProvince} {entityItem.PostalCode}&saddr=",
                        Title = "Directions",
                    });
                    card.Actions.Add(new OpenUrlAction
                    {
                        Url = $"http://maps.google.com/maps/?q={entityItem.Address1}, {entityItem.City}, {entityItem.StateProvince} {entityItem.PostalCode}",
                        Title = "View On Map",
                    });

                    Attachment attachment = new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = card
                    };

                    messageActivity.Attachments.Add(attachment);
                }
            }

            return messageActivity;
        }
    }
}