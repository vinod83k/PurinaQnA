﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Cognitive.LUIS.ActionBinding.Bot;
using PurinaQnA.Actions;
using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;

namespace PurinaQnA.Dialog
{
    [Serializable]
    public class RootDialog : LuisActionDialog<object>
    {
        public RootDialog() : base(
            new Assembly[] { typeof(GreetingsAction).Assembly },
            (action, context) =>
            {
                // Here you can implement a callback to hydrate action contexts as per request

                // For example:
                // If your action is related with a 'Booking' intent, then you could do something like:
                // BookingSystem.Hydrate(action) - hydrate action context already stored within some repository
                // (ex. using a booking ref that you can get from the context somehow)

                // To simply showcase the idea, here we are setting the checkin/checkout dates for 1 night
                // when the user starts a contextual intent related with the 'FindHotelsAction'

                // So if you simply write 'Change location to Madrid' the main action will have required parameters already set up
                // and, as in this case the context is an IDialogContext, you can get the user information for any purpose
                if (action is GreetingsAction)
                {
                    //(action as GreetingsAction).Checkin = DateTime.Today;
                    //(action as GreetingsAction).Checkout = DateTime.Today.AddDays(1);
                }
            },
            new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LUIS_ModelId"], ConfigurationManager.AppSettings["LUIS_SubscriptionKey"])))
        {
        }

        [LuisIntent("Greetings")]
        [LuisIntent("None")]
        [LuisIntent("NotHappy")]
        [LuisIntent("GoodBye")]
        public async Task GreetingsActionResultHandlerAsync(IDialogContext context, object actionResult)
        {
            // we know these actions return a string for their related intents,
            // although you could have individual handlers for each intent
            var message = context.MakeMessage();

            message.Text = actionResult != null ? actionResult.ToString() : "Cannot resolve your query";

            await context.PostAsync(message);
        }

        [LuisIntent("RetailerFinder")]
        public async Task RetailerFinderActionResultHandlerAsync(IDialogContext context, object actionResult)
        {
            // we know these actions return a string for their related intents,
            // although you could have individual handlers for each intent
            var message = context.MakeMessage();

            message.Text = actionResult != null ? actionResult.ToString() : "Cannot resolve your query";

            await context.PostAsync(message);
        }

    }
}