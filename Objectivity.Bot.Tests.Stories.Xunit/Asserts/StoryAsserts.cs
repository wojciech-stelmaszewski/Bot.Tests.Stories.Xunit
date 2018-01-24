namespace Objectivity.Bot.Tests.Stories.Xunit.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Extensions;
    using global::Xunit;
    using Player;
    using StoryModel;
    using StoryPerformer;
    using StoryPlayer;

    public class StoryAsserts
    {
        private const string NotMatchingActorMessageFormat = "Not matching actors on performance step with index = {0}. Expected actor: {1}, actual actor: {2}.";
        private const string PerformanceStepNotCoveredMessage =
            "Error while testing a story: dialog produced a step #{0} from {1} with message '{2}' which was not covered by test story.";

        private const string StoryStepNotCoveredMessage =
            "Error while testing a story: test story produced a step #{0} from {1} with message '{2}' which was not covered by performed story.";

        private readonly WrappedDialogResult wrappedDialogResult;

        public StoryAsserts(WrappedDialogResult wrappedDialogResult)
        {
            this.wrappedDialogResult = wrappedDialogResult;
        }

        public Task AssertStory(IStory story, List<PerformanceStep> performanceSteps)
        {
            var storySteps = story.StoryFrames
                .Select((storyFrame, stepIndex) => new StoryStep(storyFrame)
                {
                    Status = StoryPlayerStepStatus.NotDone,
                    StepIndex = stepIndex
                })
                .ToList();

            var stepsCount = Math.Max(storySteps.Count, performanceSteps.Count);

            for (var i = 0; i < stepsCount; i++)
            {
                this.AssertStoryStep(performanceSteps, storySteps, i);
            }

            return Task.CompletedTask;
        }

        private static string GetNotMatchingActorMessage(IStep storyStep, IStep performanceStep)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                NotMatchingActorMessageFormat,
                storyStep.StepIndex,
                storyStep.Actor,
                performanceStep.Actor);
        }

        private static string GetNotCoveredStoryStepMessage(string format, IStep step)
        {
            return string.Format(CultureInfo.InvariantCulture, format, step?.StepIndex, step?.Actor, step?.Message);
        }

        private void AssertStoryStep(
            List<PerformanceStep> performanceSteps,
            IReadOnlyList<StoryStep> storySteps,
            int stepIndex)
        {
            var storyStep = storySteps.Count > stepIndex ? storySteps[stepIndex] : null;
            var performanceStep = performanceSteps.Count > stepIndex ? performanceSteps[stepIndex] : null;

            if (storyStep == null)
            {
                Assert.True(false, GetNotCoveredStoryStepMessage(PerformanceStepNotCoveredMessage, performanceStep));
            }

            if (performanceStep == null && !storyStep.IsDialogResultCheckupStep)
            {
                Assert.True(false, GetNotCoveredStoryStepMessage(StoryStepNotCoveredMessage, storyStep));
            }

            if (storyStep.IsDialogResultCheckupStep)
            {
                FinishStepAsserts.AssertDialogFinishStep(storyStep, this.wrappedDialogResult);
            }
            else
            {
                Assert.True(storyStep != null && storyStep.Actor == performanceStep?.Actor, GetNotMatchingActorMessage(storyStep, performanceStep));

                var options = performanceSteps.TryGetOptions(stepIndex);

                StepAsserts.AssertStep(storyStep, performanceStep, options);
            }
        }
    }
}
