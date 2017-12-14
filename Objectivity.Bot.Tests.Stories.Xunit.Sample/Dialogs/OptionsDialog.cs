namespace Objectivity.Bot.Tests.Stories.Xunit.Sample.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;

    [Serializable]
    public class OptionsDialog : IDialog<object>
    {
        private static string[] Options => new[] { "Banana", "Apple", "Orange" };

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Which fruit you take?");

            var optionsMessage = this.GetOptionsMessage(context);

            await context.PostAsync(optionsMessage);

            context.Wait(this.ResumeAfterOptionMessage);
        }

        private static List<CardAction> GetButtons()
        {
            var buttons = new List<CardAction>();

            foreach (var option in Options)
            {
                buttons.Add(new CardAction
                {
                    Value = option
                });
            }

            return buttons;
        }

        private async Task ResumeAfterOptionMessage(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;

            await context.PostAsync("Your choice: " + message.Text);
            await context.PostAsync("Which fruit you take now?");

            context.Wait(this.ResumeAfterOptionMessage);
        }

        private IMessageActivity GetOptionsMessage(IDialogContext context)
        {
            var message = context.MakeMessage();
            var buttons = GetButtons();
            var card = new HeroCard { Buttons = buttons };

            var attachment = new Attachment
            {
                Content = JObject.FromObject(card)
            };

            message.Attachments = new List<Attachment> { attachment };

            return message;
        }
    }
}
