namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    using Microsoft.Bot.Builder.Dialogs;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    public interface ILuisAction
    {
        Task<object> FulfillAsync(IDialogContext context = null, string messasgeText = "");

        bool IsValid(out ICollection<ValidationResult> results);
    }
}
