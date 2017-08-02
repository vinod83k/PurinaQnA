using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Reflection;
using System.Linq;
using PurinaQnA.QnAMaker;
using System.Threading;
using PurinaQnA.Actions;

namespace PurinaQnA.Dialog
{
    /// <summary>
    /// A dialog specialized to handle QnA response from QnA Maker.
    /// </summary>
    [Serializable]
    public class QnAMakerDialog : IDialog<bool>
    {
        protected readonly IQnAService[] services;
        private QnAMakerResults qnaMakerResults;
        //private FeedbackRecord feedbackRecord;
        private const double QnAMakerHighConfidenceScoreThreshold = 0.99;
        private const double QnAMakerHighConfidenceDeltaThreshold = 0.20;

        public IQnAService[] MakeServicesFromAttributes()
        {
            var type = this.GetType();
            var qnaModels = type.GetCustomAttributes<QnAMakerAttribute>(inherit: true);
            return qnaModels.Select(m => new QnAMakerService(m)).Cast<IQnAService>().ToArray();
        }

        /// <summary>
        /// Construct the QnA Service dialog.
        /// </summary>
        /// <param name="services">The QnA service.</param>
        public QnAMakerDialog(params IQnAService[] services)
        {
            if (services.Length == 0)
            {
                services = MakeServicesFromAttributes();
            }

            SetField.NotNull(out this.services, nameof(services), services);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            
            if (message != null && !string.IsNullOrEmpty(message.Text))
            {
                var tasks = this.services.Select(s => s.QueryServiceAsync(message.Text)).ToArray();
                await Task.WhenAll(tasks);

                if (tasks.Any())
                {
                    var sendDefaultMessageAndWait = true;
                    qnaMakerResults = tasks.First(x => x.Result.ServiceCfg != null).Result;
                    if (tasks.Count(x => x.Result.Answers?.Count > 0) > 0)
                    {
                        var maxValue = tasks.Max(x => x.Result.Answers[0].Score);
                        qnaMakerResults = tasks.First(x => x.Result.Answers[0].Score == maxValue).Result;

                        if (qnaMakerResults != null && qnaMakerResults.Answers != null && qnaMakerResults.Answers.Count > 0)
                        {
                            if (this.IsConfidentAnswer(qnaMakerResults))
                            {
                                await this.RespondFromQnAMakerResultAsync(context, message, qnaMakerResults);
                                await this.DefaultWaitNextMessageAsync(context, message, qnaMakerResults);
                            }
                            else
                            {
                                //feedbackRecord = new FeedbackRecord { UserId = message.From.Id, UserQuestion = message.Text };
                                await this.QnAFeedbackStepAsync(context, qnaMakerResults);
                            }

                            sendDefaultMessageAndWait = false;
                        }
                    }

                    if (sendDefaultMessageAndWait)
                    {
                        await context.PostAsync(qnaMakerResults.ServiceCfg.DefaultMessage);
                        await this.DefaultWaitNextMessageAsync(context, message, qnaMakerResults);
                    }
                }
            }
        }

        protected virtual bool IsConfidentAnswer(QnAMakerResults qnaMakerResults)
        {
            if (qnaMakerResults.Answers.Count <= 1 || qnaMakerResults.Answers.First().Score >= QnAMakerHighConfidenceScoreThreshold)
            {
                return true;
            }

            if (qnaMakerResults.Answers[0].Score - qnaMakerResults.Answers[1].Score > QnAMakerHighConfidenceDeltaThreshold)
            {
                return true;
            }

            return false;
        }

        private async Task ResumeAndPostAnswer(IDialogContext context, IAwaitable<string> argument)
        {
            try
            {
                var selection = await argument;
                if (qnaMakerResults != null)
                {
                    bool match = false;
                    foreach (var qnaMakerResult in qnaMakerResults.Answers)
                    {
                        if (qnaMakerResult.Questions[0].Equals(selection, StringComparison.CurrentCultureIgnoreCase))
                        {
                            await context.PostAsync(qnaMakerResult.Answer);
                            match = true;
                            break;
                        }
                        else if (Resources.ChatBot.noneOfTheAboveOption.Equals(selection, StringComparison.OrdinalIgnoreCase))
                        {
                            await context.PostAsync(Resources.ChatBot.NoMatchMessage);
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                    {
                        await context.Forward(new RootDialog(), ResumeAfterRootDialog, selection, CancellationToken.None);
                        context.Done(selection);
                    }
                }
            }
            catch (TooManyAttemptsException) {
                await context.Forward(new RootDialog(), ResumeAfterRootDialog, context.Activity.AsMessageActivity(), CancellationToken.None);
                context.Done("");
            }
            await this.DefaultWaitNextMessageAsync(context, context.Activity.AsMessageActivity(), qnaMakerResults);
        }

        private async Task ResumeAfterRootDialog(IDialogContext context, IAwaitable<object> result)
        {
        }

        protected virtual async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            var qnaList = qnaMakerResults.Answers;
            //var questions = qnaList.Select(x => x.Questions[0]).Concat(new[] {Resources.ChatBot.noneOfTheAboveOption}).ToArray();
            var questions = qnaList.Select(x => x.Questions[0]).ToArray();

            PromptOptions<string> promptOptions = new PromptOptions<string>(
                prompt: Resources.ChatBot.answerSelectionPrompt,
                tooManyAttempts: Resources.ChatBot.tooManyAttempts,
                options: questions,
                attempts: 0);

            QnAMakerPromptChoice.Choice(context, ResumeAndPostAnswer, promptOptions);
            //PromptDialog.Choice(context: context, resume: ResumeAndPostAnswer, promptOptions: promptOptions);
        }

        protected virtual async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            await context.PostAsync(result.Answers.First().Answer);
        }

        protected virtual async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            context.Done(true);
        }
    }
}
