namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPerformer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StoryModel;

    public interface IStoryPerformer
    {
        Task<List<PerformanceStep>> Perform(IStory testStory);
    }
}
