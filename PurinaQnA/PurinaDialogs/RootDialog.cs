using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using PurinaQnA.Dialogs;
using System.Threading;

namespace PurinaQnA.PurinaDialogs
{
    #pragma warning disable 1998

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            try
            {
                if (message.Text.ToLower().Contains("retailer finder") || message.Text.ToLower().Contains("retailer"))
                {
                    try
                    {
                        await context.Forward(Chain.From(() => FormDialog.FromForm(RetailerFinderFormDialog.BuildForm)), ResumeAfterFormDialog, message, CancellationToken.None);
                    }
                    catch (Exception)
                    {
                        await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity.AsMessageActivity(), $"We could not find retailers for your location. Please try with another Zip/City/State by selecting 'Retailer Finder'."));
                    }
                }
                else if (message.Text.ToLower().Contains("faq"))
                {
                    await context.PostAsync(MessageUtility.GetSimpleTextMessage(context.MakeMessage(), "Please enter your question"));
                    await context.Forward(new BasicQnAMakerDialog(), ResumeAfterQnaMakerDialog, message, CancellationToken.None);
                }
                else
                {
                    await context.Forward(new SimpleMessageDialog(), ResumeAfterSimpleMessageDialog, message, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {

            }

        }

        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<object> result)
        {
            RetailerFinderFormDialog msgResult = null;
            try
            {
                msgResult = (RetailerFinderFormDialog)await result;
            }
            catch (Exception)
            {
            }

            if (msgResult != null && msgResult.ResultCount > 0)
            {
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity.AsMessageActivity(), "Hope you find your retailers. Please select if you are looking for FAQ"));
            }
            else
            {
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity.AsMessageActivity(), $"We could not find retailers for your location. Please try with another Zip/City/State by selecting 'Retailer Finder'."));
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterQnaMakerDialog(IDialogContext context, IAwaitable<object> result)
        {
            var messageResult = await result;
            if (messageResult.ToString().ToLower().Contains("nomatch"))
            {
                var messageActivity = context.MakeMessage();
                await context.PostAsync(MessageUtility.GetSimpleTextMessage(messageActivity, "We could not find answer to your question"));

                //await context.PostAsync(GetMessage(context, ""));
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity, "Please click on the below options"));
                context.Done("null");
            }
            else if (messageResult.ToString().ToLower().Contains("retailer"))
            {
                await context.PostAsync(MessageUtility.GetWelcomeOptionsMessage((Activity)context.Activity, "Looking for"));
                context.Done("null");
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSimpleMessageDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Hey, I got your message!");
            context.Wait(MessageReceivedAsync);
        }
    }
}