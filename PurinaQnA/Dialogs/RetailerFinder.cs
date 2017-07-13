using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    public class RetailerFinder
    {
        private const int TakeResultCount = 5;
        public double ResultCount { get; private set; }

        [Prompt("Please enter either ZipCode, City or State")]
        public string Location;

        public static IForm<RetailerFinder> BuildForm()
        {
            OnCompletionAsyncDelegate<RetailerFinder> processOrder = async (context, state) =>
            {
                // call service api and get the Retailers
                var retailers = await GetRetailerFinderApiResponse(state.Location);
                var messageActivity = CreateRetailerMessage(context, retailers);
                state.ResultCount = retailers.ResultCount;
                if (retailers.ResultCount > TakeResultCount)
                {

                    await context.PostAsync(MessageUtility.GetSimpleTextMessage(context.MakeMessage(), $"Here is the list of top {TakeResultCount} nearest locations..."));
                }
                await context.PostAsync(messageActivity);
            };

            return new FormBuilder<RetailerFinder>()
                .Field(nameof(Location))
                .OnCompletion(processOrder)                
                .Build();
        }

        private static async Task<RetailerFinderEntity> GetRetailerFinderApiResponse(string location) {
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

        private static IMessageActivity CreateRetailerMessage(IDialogContext context, RetailerFinderEntity entity) {
            var messageActivity = context.MakeMessage();
            messageActivity.Attachments = new List<Attachment>();

            if (entity != null && entity.EntityList.Any()) {

                foreach (var entityItem in entity.EntityList.OrderBy(x => x.Distance).Take(TakeResultCount)) {

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

    public class RetailerFinderEntity {

        [JsonProperty(PropertyName = "ENTITY_LIST")]
        public List<RetailerFinderApiResult> EntityList { get; set; }

        [JsonProperty(PropertyName = "RESULT_COUNT")]
        public double ResultCount { get; set; }
    }

    public class RetailerFinderApiResult
    {
        [JsonProperty(PropertyName = "NAME")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "ADDRESS1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "ADDRESS2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "CITY")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "STATE_PROVINCE")]
        public string StateProvince { get; set; }

        [JsonProperty(PropertyName = "EMAIL_ADDRESS")]
        public string EmailAddress { get; set; }

        [JsonProperty(PropertyName = "PHONE_NUMBER")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "POSTAL_CODE")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "DISTANCE")]
        public double Distance { get; set; }

    }
}