namespace Objectivity.Bot.Tests.Stories.Xunit.Sample.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class UserInitiatedDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.ResumeAfterFirstMessage);

            return Task.CompletedTask;
        }

        private async Task ResumeAfterFirstMessage(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;

            await context.PostAsync("You said: " + message.Text);

            context.Wait(this.ResumeAfterLastMessage);
        }

        private async Task ResumeAfterLastMessage(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;

            await context.PostAsync("You said: " + message.Text);

            context.Done<object>(null);
        }
    }
}
