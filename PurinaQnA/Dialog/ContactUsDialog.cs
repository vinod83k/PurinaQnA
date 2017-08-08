using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AdaptiveCards;
using System.Threading;
using PurinaQnA.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PurinaQnA.Dialog
{
    public class ContactUsDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await ShowContactUsOptionsAsync(context);
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message != null)
            {
                if (message.Value != null)
                {
                    string responseMsg = Resources.ChatBot.WhatElseHelp;
                    dynamic value = message.Value;
                    string submitType = value.Type.ToString();
                    if (submitType.Equals("ContactUs", StringComparison.CurrentCultureIgnoreCase))
                    {
                        try
                        {
                            var model = ContactUsModel.Parse(value);
                            if (model != null)
                            {
                                // Trigger validation using Data Annotations attributes from the ContactUs model
                                List<ValidationResult> results = new List<ValidationResult>();
                                bool valid = Validator.TryValidateObject(model, new ValidationContext(model, null, null), results, true);
                                if (!valid)
                                {
                                    // Some field in the ContactUs details are not valid
                                    var errors = string.Join("\n", results.Select(o => " - " + o.ErrorMessage));
                                    await context.PostAsync($"{Resources.ChatBot.ContactUsValidationTitle}:\n" + errors);
                                    return;
                                }

                                responseMsg = string.Format(Resources.ChatBot.ContactUsWithDetailsMsg, model.Name, model.EmailAddress);
                            }
                        }
                        catch (Exception)
                        {
                            await context.PostAsync(Resources.ChatBot.ContactUsValidationTitle);
                            return;
                        }
                    }

                    await context.PostAsync(responseMsg);
                    context.Done(true);
                }
                else
                {
                    context.Done(false);
                }
            }
        }

        private async Task ShowContactUsOptionsAsync(IDialogContext context)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                //Body = new List<CardElement>()
                //{
                //    new TextBlock()
                //    {
                //        Text = Resources.ChatBot.ContactUsOptionsMsg,
                //        Weight = TextWeight.Normal,
                //        Size = TextSize.Small,
                //        Color = TextColor.Dark,
                //        Wrap = true,
                //        IsSubtle = true
                //    }
                //},
                // Buttons
                Actions = new List<ActionBase>() {
                    new ShowCardAction()
                    {
                        Title = Resources.ChatBot.ContactUs,
                        Speak = "<s>" + Resources.ChatBot.ContactUs +"</s>",
                        Card = GetContactUsCard()
                    },
                    new OpenUrlAction {
                        Title = Resources.ChatBot.AskOurExperts,
                        Speak = "<s>" + Resources.ChatBot.AskOurExperts +"</s>",
                        Url = "https://www.purinamills.com/ask-an-expert"
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(Resources.ChatBot.ContactUsOptionsMsg);
            await context.PostAsync(reply, CancellationToken.None);

        }

        private static AdaptiveCard GetContactUsCard()
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                        new TextBlock() { Text = Resources.ChatBot.WhatYouWouldLikeToKnow, Wrap = true, Weight = TextWeight.Bolder, Color = TextColor.Accent },
                        new TextBlock() { Text = Resources.ChatBot.ContactUsSubTitle, Wrap = true },
                        new TextBlock() { Text = Resources.ChatBot.Species },
                        new ChoiceSet {
                            Style = ChoiceInputStyle.Compact,
                            Id = "Species",
                            IsRequired = true,
                            Value="Species",                        
                            Choices = new List<Choice> {
                                new Choice { Title = "Horses", Value = "Horses" },
                                new Choice { Title = "Cattle", Value = "Cattle" },
                                new Choice { Title = "BackYard Poultry", Value = "BackYard Poultry" },
                                new Choice { Title = "Dairy", Value = "Dairy" },
                                new Choice { Title = "Goat", Value = "Goat" },
                                new Choice { Title = "Swine", Value = "Swine" },
                                new Choice { Title = "Rabbits", Value = "Rabbits" },
                                new Choice { Title = "Deer", Value = "Deer" },
                                new Choice { Title = "Fish & Aquatics", Value = "Fish & Aquatics" },
                                new Choice { Title = "Show Animals", Value = "Show Animals" },
                                new Choice { Title = "Birds", Value = "Birds" }
                            }
                        },
                        new TextBlock() { Text = Resources.ChatBot.WhatIsQuestion },
                        new TextInput()
                        {
                            Id = "Question",
                            Speak = "<s>"+ Resources.ChatBot.WhatIsQuestion +"</s>",
                            Style = TextInputStyle.Text,
                            IsMultiline = true,
                            IsRequired = true
                        },
                        new TextBlock() { Text = Resources.ChatBot.NameRequiredMessage },
                        new TextInput()
                        {
                            Id = "Name",
                            Speak = "<s>"+ Resources.ChatBot.NameRequiredMessage +"</s>",
                            Style = TextInputStyle.Text,
                            IsMultiline = false,
                            IsRequired = true,
                            MaxLength = 20
                        },
                        new TextBlock() { Text = Resources.ChatBot.EmailAddressRequiredMessage },
                        new TextInput()
                        {
                            Id = "EmailAddress",
                            Speak = "<s>"+ Resources.ChatBot.EmailAddressRequiredMessage +"</s>",
                            Style = TextInputStyle.Text,
                            IsMultiline = false,
                            IsRequired = true,
                            MaxLength = 50
                        },
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Submit",
                        Speak = "<s>Submit</s>",
                        DataJson = "{ \"Type\": \"ContactUs\" }"
                    }
                }
            };
        }

    }
}