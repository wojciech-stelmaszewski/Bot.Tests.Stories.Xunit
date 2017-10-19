namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Container;
    using Core;
    using global::Xunit;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;
    using Player;
    using StoryModel;

    internal class UnitTestStoryPlayer : IStoryPlayer
    {
        private readonly IDictionary<string, object> outputValues = new Dictionary<string, object>();
        private readonly Queue<IMessageActivity> receivedMessages = new Queue<IMessageActivity>();
        private readonly string userId = Guid.NewGuid().ToString();
        private readonly ITestContainerBuilder testContainerBuilder;

        private IDialog<object> testDialog;
        private string[] latestOptions;
        private IMessageActivity toBotMessageActivity;

        public UnitTestStoryPlayer(ITestContainerBuilder testContainerBuilder)
        {
            this.testContainerBuilder = testContainerBuilder;
        }

        public UnitTestStoryPlayer(IDialog<object> testDialog, ITestContainerBuilder testContainerBuilder)
        {
            this.testDialog = testDialog;
            this.testContainerBuilder = testContainerBuilder;
        }

        public async Task<IStoryResult> Play(IStory story, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.InitializeToBotMessageActivity();
            this.receivedMessages.Clear();

            using (var container = this.GetDialogTestContainer())
            using (var scope = DialogModule.BeginLifetimeScope(container, this.toBotMessageActivity))
            {
                if (this.testDialog == null)
                {
                    this.testDialog = container.Resolve<IDialog<object>>();
                }

                DialogModule_MakeRoot.Register(scope, this.RootDialog);

                var steps = story.StoryFrames.Select(storyFrame => new StoryPlayerStep
                    {
                        StoryFrame = storyFrame,
                        Status = StoryPlayerStepStatus.NotDone
                    })
                    .ToArray();

                foreach (var step in steps)
                {
                    await this.Process(step, scope);
                }

                if (steps.Last().StoryFrame.Actor == Actor.Bot)
                {
                    Assert.Empty(this.receivedMessages);
                }
            }

            return this.GetResult();
        }

        private IStoryResult GetResult()
        {
            var result = new StoryResult();

            foreach (var pair in this.outputValues)
            {
                result.OutputValues.Add(pair.Key, pair.Value);
            }

            return result;
        }

        private IContainer GetDialogTestContainer()
        {
            using (new FiberTestBase.ResolveMoqAssembly(this.testDialog))
            {
                var options = TestContainerBuilderOptions.MockConnectorFactory
                    | TestContainerBuilderOptions.ScopedQueue
                    | TestContainerBuilderOptions.ResolveDialogFromContainer;

                if (this.testDialog != null)
                {
                    return this.testContainerBuilder.Build(options, this.testDialog);
                }
                else
                {
                    return this.testContainerBuilder.Build(options);
                }
            }
        }

        private void InitializeToBotMessageActivity()
        {
            this.toBotMessageActivity = DialogTestBase.MakeTestMessage();
            this.toBotMessageActivity.From.Id = this.userId;
        }

        private async Task Process(StoryPlayerStep step, ILifetimeScope scope)
        {
            this.ReadResponses(scope);

            var storyFrame = step.StoryFrame;

            switch (storyFrame.Actor)
            {
                case Actor.Bot:
                    this.HandleBotStoryMessage(storyFrame);
                    break;
                case Actor.User:
                    await this.HandleUserStoryMessage(scope, storyFrame);
                    break;
            }
        }

        private void HandleBotStoryMessage(IStoryFrame storyFrame)
        {
            var message = this.receivedMessages.Dequeue();

            switch (storyFrame.ComparisonType)
            {
                case ComparisonType.TextExact:
                    this.ProcessBotFrameTextExact(storyFrame, message);
                    break;

                case ComparisonType.TextMatchRegex:
                    this.ProcessBotFrameTextMatchRegex(storyFrame, message);
                    break;

                case ComparisonType.AttachmentListPresent:
                    this.ProcessBotFrameListPresent(storyFrame, message);
                    break;
            }
        }

        private void ProcessBotFrameTextExact(IStoryFrame storyFrame, IMessageActivity message)
        {
            Assert.NotNull(message);
            Assert.Equal("message", message.Type);
            Assert.Equal(storyFrame.Text, message.Text);
        }

        private void ProcessBotFrameTextMatchRegex(IStoryFrame storyFrame, IMessageActivity message)
        {
            Assert.NotNull(message);
            Assert.Equal("message", message.Type);
            Assert.Matches(storyFrame.Text, message.Text);
        }

        private void ProcessBotFrameListPresent(IStoryFrame storyFrame, IMessageActivity message)
        {
            Assert.NotNull(message);
            Assert.Equal("list", message.AttachmentLayout);
            Assert.Equal(1, message.Attachments.Count);

            var listJson = (JObject)message.Attachments[0].Content;
            if (storyFrame.ListPredicate != null)
            {
                Assert.True(storyFrame.ListPredicate(listJson), "List contains expected item");
            }

            this.latestOptions = listJson.SelectToken("buttons").Select(item => item["value"].ToString()).ToArray();
        }

        private async Task HandleUserStoryMessage(ILifetimeScope scope, IStoryFrame storyFrame)
        {
            switch (storyFrame.ComparisonType)
            {
                case ComparisonType.TextExact:
                    await this.ProcessUserFrameTextExact(scope, storyFrame);
                    break;
                case ComparisonType.Option:
                    await this.ProcessUserFrameOption(scope, storyFrame);
                    break;
            }
        }

        private async Task ProcessUserFrameTextExact(ILifetimeScope scope, IStoryFrame storyStoryFrame)
        {
            await this.SendUserMessage(scope, storyStoryFrame.Text);
        }

        private async Task ProcessUserFrameOption(ILifetimeScope scope, IStoryFrame storyFrame)
        {
            Assert.NotEmpty(this.latestOptions);

            var optionValue = this.latestOptions[storyFrame.OptionIndex];

            if (!string.IsNullOrEmpty(storyFrame.OptionOutputPlaceholder))
            {
                this.outputValues.Add(storyFrame.OptionOutputPlaceholder, optionValue);
            }

            await this.SendUserMessage(scope, optionValue);
        }

        private async Task SendUserMessage(ILifetimeScope scope, string message)
        {
            this.toBotMessageActivity.Text = message;

            await TestConversation.SendAsync(scope, this.toBotMessageActivity);
        }

        private void ReadResponses(ILifetimeScope scope)
        {
            var queue = scope.Resolve<Queue<IMessageActivity>>();

            while (queue.Any())
            {
                var message = queue.Dequeue();

                if (message.Type != "typing")
                {
                    this.receivedMessages.Enqueue(message);
                }
            }
        }

        private IDialog<object> RootDialog()
        {
            return this.testDialog;
        }
    }
}
