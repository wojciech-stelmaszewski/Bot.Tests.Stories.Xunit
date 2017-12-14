namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPerformer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Extensions;
    using IO;
    using Player;
    using Recorder;
    using StoryModel;
    using StoryPlayer;

    public class WrappedStoryPerformer : IStoryPerformer
    {
        private readonly IConversationService conversationService;
        private readonly IDialogReader dialogReader;
        private readonly IDialogWriter dialogWriter;
        private readonly IPerformanceStory performanceStory;

        public WrappedStoryPerformer(IScopeContext scopeContext, IConversationService conversationService)
        {
            this.conversationService = conversationService;

            this.performanceStory = new PerformanceStory();
            this.dialogReader = new WrapperDialogReader(scopeContext);
            this.dialogWriter = new WrappedDialogWriter(scopeContext, conversationService);
        }

        public async Task<List<PerformanceStep>> Perform(IStory testStory)
        {
            var steps = this.GetStorySteps(testStory);

            foreach (var step in steps)
            {
                this.PushStartupMessageActivities();

                await this.WriteUserMessageActivity(step);

                this.ReadBotMessageActivities();

                this.TrySetLatestOptions();
            }

            return this.performanceStory.Steps;
        }

        private List<StoryStep> GetStorySteps(IStory testStory)
        {
            var wrapperStory = StoryRecorder.Record()
                .User.Says(Consts.WrapperStartMessage)
                .Bot.Says(Consts.WrapperStartMessage)
                .Rewind();

            var wrappedStory = testStory.Concat(wrapperStory);

            return wrappedStory.StoryFrames.Select((storyFrame, stepIndex) =>
                new StoryStep(storyFrame)
                {
                    Status = StoryPlayerStepStatus.NotDone,
                    StepIndex = stepIndex
                })
                .ToList();
        }

        private void PushStartupMessageActivities()
        {
            this.performanceStory.PushStartupSteps();
        }

        private void ReadBotMessageActivities()
        {
            var startupMessageActivities = this.dialogReader.DequeueStartupMessageActivities();
            var messageActivities = this.dialogReader.GetMessageActivities();

            this.performanceStory.EnqueueStartupSteps(startupMessageActivities, Actor.Bot);
            this.performanceStory.AddSteps(messageActivities, Actor.Bot);
        }

        private void TrySetLatestOptions()
        {
            var options = this.performanceStory.Steps.TryGetOptions();
            this.conversationService.LatestOptions = options;
        }

        private async Task WriteUserMessageActivity(StoryStep step)
        {
            if (step.Actor == Actor.User)
            {
                var activity = await this.dialogWriter.SentActivity(step.StoryFrame);
                this.performanceStory.AddStep(activity, Actor.User);
            }
        }
    }
}
