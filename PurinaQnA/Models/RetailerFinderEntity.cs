using Newtonsoft.Json;
using System.Collections.Generic;

namespace PurinaQnA.Models
{
    public class RetailerFinderEntity
    {

        [JsonProperty(PropertyName = "ENTITY_LIST")]
        public List<RetailerFinderApiResult> EntityList { get; set; }

        [JsonProperty(PropertyName = "RESULT_COUNT")]
        public double ResultCount { get; set; }
    }
}