namespace Objectivity.Bot.Tests.Stories.Xunit.Extensions
{
    using System.Linq;
    using StoryModel;

    public static class StoryExtensions
    {
        public static IStory Concat(this IStory firstStory, IStory secondStory)
        {
            var frames = secondStory.StoryFrames.Concat(firstStory.StoryFrames);
            var story = new Story();

            foreach (var frame in frames)
            {
                story.AddStoryFrame(frame);
            }

            return story;
        }
    }
}
