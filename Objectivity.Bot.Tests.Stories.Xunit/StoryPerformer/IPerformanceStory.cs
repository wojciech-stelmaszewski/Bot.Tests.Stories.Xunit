namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPerformer
{
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;
    using StoryModel;

    public interface IPerformanceStory
    {
        List<PerformanceStep> Steps { get; set; }

        void EnqueueStartupStep(IMessageActivity messageActivity, Actor actor);

        void EnqueueStartupSteps(List<IMessageActivity> messageActivities, Actor actor);

        void AddStep(IMessageActivity messageActivity, Actor actor);

        void AddSteps(List<IMessageActivity> messageActivities, Actor actor);

        void PushStartupSteps();
    }
}
