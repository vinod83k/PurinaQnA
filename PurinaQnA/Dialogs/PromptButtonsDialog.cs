using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using PurinaQnA.Utils;
using PurinaQnA.Enumerations;
using System.Linq;

namespace PurinaQnA.Dialogs
{
    [Serializable]
    //This is actually Root Dialog of this bot, but I named PromptButtons Dialog becuase I want to set similar name in node.js sample.
    public class PromptButtonsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //Show options whatever users chat
            var option = await result;

            PromptDialog.Choice(context, 
                AfterMenuSelection, 
                new List<string> { string.Format("{0} feed", option.Text) }, 
                "Are you looking for ?");
        }

        //After users select option, Bot call other dialogs
        private async Task AfterMenuSelection(IDialogContext context, IAwaitable<string> result)
        {
            var optionSelected = await result;
            await context.Forward(new BasicQnAMakerDialog(), ResumeAfterOptionDialog, context.Activity.AsMessageActivity());            
        }

        //This function is called after each dialog process is done
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            //This means  MessageRecievedAsync function of this dialog (PromptButtonsDialog) will receive users' messeges
            context.Wait(MessageReceivedAsync);
        }
    }
}