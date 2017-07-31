namespace PurinaQnA.Actions
{
    using Dialog;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using QnAMaker;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [Serializable]
    [LuisActionBinding("AnimalsNutrition")]
    public class AnimalsAction : BaseLuisAction
    {
        private Dialog.QnAMakerDialog qnaMakerDialog;
        protected readonly IQnAService[] services;
        private QnAMakerResults qnaMakerResults;
        private FeedbackRecord feedbackRecord;
        private const double QnAMakerHighConfidenceScoreThreshold = 0.99;
        private const double QnAMakerHighConfidenceDeltaThreshold = 0.20;

        public IQnAService[] MakeServicesFromAttributes()
        {
            //var type = this.GetType();
            var qnaModels = new QnAMakerAttribute(subscriptionKey: ConfigurationManager.AppSettings["SubscriptionKey"],
                knowledgebaseId: ConfigurationManager.AppSettings["KnowledgeBaseId"],
                defaultMessage: Resources.ChatBot.NoMatchMessage,
                scoreThreshold: 0.2,
                top: 4);

            return new List<IQnAService> { new QnAMakerService(qnaModels) }.ToArray();
        }

        public AnimalsAction() {
            if (services == null)
            {
                services = MakeServicesFromAttributes();
            }

            SetField.NotNull(out this.services, nameof(services), services);
        }

        private async Task ResumeAfterQnAMakerDialog(IDialogContext context, IAwaitable<bool> result) {
            context.Done(true);
        }

        public override async Task<object> FulfillAsync(IDialogContext context = null, string messageText = "")
        {
            var message = messageText;

            if (message != null && !string.IsNullOrEmpty(message))
            {
                var tasks = this.services.Select(s => s.QueryServiceAsync(message)).ToArray();
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
                                await this.RespondFromQnAMakerResultAsync(context, context.Activity.AsMessageActivity(), qnaMakerResults);
                                await this.DefaultWaitNextMessageAsync(context, context.Activity.AsMessageActivity(), qnaMakerResults);
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
                        await this.DefaultWaitNextMessageAsync(context, context.Activity.AsMessageActivity(), qnaMakerResults);
                    }
                }
            }

            return Task.FromResult((object)"");
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
                    if (!match) {
                        await context.Forward(new RootDialog(), ResumeAfterRootDialog, selection, CancellationToken.None);
                        context.Done(selection);
                    }
                }
            }
            catch (Exception) {
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
            //var questions = qnaList.Select(x => x.Questions[0]).Concat(new[] { Resources.ChatBot.noneOfTheAboveOption }).ToArray();
            var questions = qnaList.Select(x => x.Questions[0]).ToArray();

            PromptOptions<string> promptOptions = new PromptOptions<string>(
                prompt: Resources.ChatBot.answerSelectionPrompt,
                tooManyAttempts: "",
                options: questions,
                attempts: 0);

            PromptDialog.Choice(context: context, resume: ResumeAndPostAnswer, promptOptions: promptOptions);
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