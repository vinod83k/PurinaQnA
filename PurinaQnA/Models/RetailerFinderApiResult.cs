using Newtonsoft.Json;

namespace PurinaQnA.Models
{
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