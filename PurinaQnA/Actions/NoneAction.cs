﻿namespace PurinaQnA.Actions
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("None")]
    public class NoneAction : BaseLuisContextualAction<AnimalsAction>
    {
        public override async Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            return Task.FromResult((object)"");
        }
    }
}