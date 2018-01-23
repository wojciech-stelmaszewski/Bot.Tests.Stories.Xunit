namespace Objectivity.Bot.Tests.Stories.Xunit.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Objectivity.Bot.Tests.Stories.Xunit.Core;
    using StoryPerformer;

    [Serializable]
    internal class WrapperDialog : IDialog<object>
    {
        private readonly IDialog<object> wrappedDialog;

        public WrapperDialog(IDialog<object> wrappedDialog, WrappedDialogResult wrappedDialogResult)
        {
            this.wrappedDialog = wrappedDialog;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Consts.WrapperStartMessage);

            context.Wait(this.ResumeAfterStart);
        }

        private Task ResumeAfterStart(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            context.Call(this.wrappedDialog, this.ResumeAfterFinish);

            return Task.CompletedTask;
        }

        private Task ResumeAfterFinish(IDialogContext context, IAwaitable<object> item)
        {
            return Task.CompletedTask;
        }
    }
}
