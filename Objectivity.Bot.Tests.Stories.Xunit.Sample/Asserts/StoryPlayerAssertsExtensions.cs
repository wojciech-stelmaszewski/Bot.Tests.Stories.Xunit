namespace Objectivity.Bot.Tests.Stories.Xunit.Sample.Asserts
{
    using System.Threading.Tasks;
    using global::Xunit;
    using global::Xunit.Sdk;
    using Player;
    using StoryModel;

    public static class StoryPlayerAssertsExtensions
    {
        public static async Task ThrowsTrueException(this IStoryPlayer storyPlayer, IStory story, string messagePattern = null)
        {
            async Task TestCode()
            {
                await storyPlayer.Play(story);
            }

            // ReSharper disable once PossibleNullReferenceException
            var exception = await Record.ExceptionAsync(TestCode);

            Assert.NotNull(exception);
            Assert.IsType<TrueException>(exception);

            if (!string.IsNullOrEmpty(messagePattern))
            {
                Assert.Matches(messagePattern, exception.Message);
            }
        }
    }
}
