using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PurinaQnA.Actions
{
    [Serializable]
    public class QnAMakerPromptChoice : PromptDialog.PromptChoice<string>
    {
        protected readonly PromptOptions<string> _qnaMakerPromptOptions;

        public QnAMakerPromptChoice(PromptOptions<string> qnaMakerPromptOptions) : base(qnaMakerPromptOptions) {
            _qnaMakerPromptOptions = qnaMakerPromptOptions;
        }

        public QnAMakerPromptChoice(IEnumerable<string> options, string prompt, string cancelPrompt, string retry, int attempts, PromptStyle promptStyle = PromptStyle.Auto)
            : this(new PromptOptions<string>(prompt, cancelPrompt, retry, options: options.ToList(), attempts: attempts, promptStyler: new PromptStyler(promptStyle)))
        {
        }

        public static void Choice(IDialogContext context, ResumeAfter<string> resume, IEnumerable<string> options, string prompt, string cancelPrompt = null, string retry = null, int attempts = 3, PromptStyle promptStyle = PromptStyle.Auto)
        {
            Choice(context, resume, new PromptOptions<string>(prompt, cancelPrompt, retry, attempts: attempts, options: options.ToList(), promptStyler: new PromptStyler(promptStyle)));
        }

        public static void Choice(IDialogContext context, ResumeAfter<string> resume, PromptOptions<string> promptOptions)
        {
            var child = new QnAMakerPromptChoice(promptOptions);
            context.Call(child, resume);
        }


        public bool IsQnaMakerOption(string text)
        {
            return _qnaMakerPromptOptions.Options.Any(t => string.Equals(t, text, StringComparison.CurrentCultureIgnoreCase));
        }

        protected override bool TryParse(IMessageActivity message, out string result)
        {
            result = message.Text;
            return IsQnaMakerOption(message.Text);
        }
    }
}