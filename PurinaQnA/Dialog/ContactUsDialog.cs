using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AdaptiveCards;
using System.Threading;
using PurinaQnA.Models;

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
                        var model = ContactUsModel.Parse(value);
                        if (model != null)
                        {
                            responseMsg = string.Format(Resources.ChatBot.ContactUsWithDetailsMsg, model.Name, model.EmailAddress);
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
                Body = new List<CardElement>()
                {
                    new Container()
                    {
                        Speak = "<s>" + Resources.ChatBot.ContactUsOptionsMsg + "</s>",
                        Items = new List<CardElement>()
                        {
                            new ColumnSet()
                            {
                                Columns = new List<Column>()
                                {
                                    new Column()
                                    {
                                        Size = ColumnSize.Stretch,
                                        Items = new List<CardElement>()
                                        {
                                            new TextBlock()
                                            {
                                                Text = Resources.ChatBot.ContactUsOptionsMsg,
                                                Weight = TextWeight.Normal,
                                                Size = TextSize.Medium,
                                                Wrap = true,
                                                IsSubtle = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
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

            await context.PostAsync(reply, CancellationToken.None);

        }

        private static AdaptiveCard GetContactUsCard()
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
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