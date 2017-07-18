namespace PurinaQnA.Actions
{
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("RetailerFinder")]
    public class RetailerFinderAction : BaseLuisAction
    {
        [Required(ErrorMessage = "Please provide a location")]
        //[Location(ErrorMessage = "Please provide a valid location")]
        [LuisActionBindingParam(BuiltinType = BuiltInGeographyTypes.City, Order = 1)]
        public string Location { get; set; }

        public override Task<object> FulfillAsync()
        {
            return Task.FromResult((object)$"Here are the top 5 retailers near to {this.Location}.");
        }
    }
}