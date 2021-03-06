﻿namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPerformer.IO
{
    using System.Threading.Tasks;
    using Core;
    using Exceptions;
    using Microsoft.Bot.Connector;
    using Moq;
    using StoryModel;

    public class WrappedDialogWriter : IDialogWriter
    {
        private readonly IScopeContext scopeContext;
        private readonly IConversationService conversationService;

        public WrappedDialogWriter(IScopeContext scopeContext, IConversationService conversationService)
        {
            this.scopeContext = scopeContext;
            this.conversationService = conversationService;
        }

        public async Task SendActivity(IMessageActivity messageActivity)
        {
            try
            {
                await TestConversation.SendAsync(this.scopeContext.Scope, messageActivity);
            }
            catch (MockException mockException)
            {
                var message = messageActivity?.Text;

                throw new UnmatchedUtteranceException(
                    $"Error while sending user message - matching intent couldn't be found. Check if unit test registers intent for the following utterance: {message}.",
                    mockException);
            }
        }

        public async Task<IMessageActivity> GetStepMessageActivity(IStoryFrame frame)
        {
            var message = this.GetUserStepMessage(frame);

            return this.conversationService.GetToBotActivity(message);
        }

        private string GetUserStepMessage(IStoryFrame frame)
        {
            switch (frame.ComparisonType)
            {
                case ComparisonType.Option:
                    return this.GetUserOptionMessage(frame);
                default:
                    return frame.Text;
            }
        }

        private string GetUserOptionMessage(IStoryFrame frame)
        {
            if (this.conversationService.LatestOptions == null)
            {
                throw new MissingOptionsException();
            }

            var index = frame.OptionIndex;

            if (index < 0 || this.conversationService.LatestOptions.Length <= index)
            {
                throw new OptionsIndexOutOfRangeException(index, this.conversationService.LatestOptions.Length);
            }

            return this.conversationService.LatestOptions[index];
        }
    }
}
