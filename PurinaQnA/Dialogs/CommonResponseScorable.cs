using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Scorables.Internals;
using PurinaQnA.Utils;
using System.Linq;

namespace PurinaQnA.Dialogs
{
#pragma warning disable 1998

    public class CommonResponseScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask task;

        public CommonResponseScorable(IDialogTask task)
        {
            SetField.NotNull(out this.task, nameof(task), task);
        }

        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken token)
        {
            var message = activity as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (CommonResponseKeywords.Keywords.Any(x => x.Equals(message.Text, StringComparison.InvariantCultureIgnoreCase))) {
                    return message.Text;
                }

                //if (message.Text.Equals("hello", StringComparison.InvariantCultureIgnoreCase))
                //{
                //}
            }

            return null;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null)
            {
                var commonResponseDialog = new CommonResponsesDialog();

                var interruption = commonResponseDialog.Void<object, IMessageActivity>();

                //this.task.Call(interruption, null);
                await this.task.Forward(interruption, null, message, token);

                await this.task.PollAsync(token);
            }
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}