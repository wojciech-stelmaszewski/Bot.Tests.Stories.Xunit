namespace Objectivity.Bot.Tests.Stories.Xunit
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Container;
    using Core;
    using Microsoft.Bot.Builder.Dialogs;
    using Player;
    using StoryModel;
    using StoryPlayer;

    public abstract class DialogUnitTestBase<TDialog> : DialogTestBase, IStoryPlayer
        where TDialog : IDialog<object>
    {
        public async Task<IStoryResult> Play(
            IStory story,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = GetTestContainerBuilder();
            var player = new UnitTestStoryPlayer(builder);

            return await player.Play(story, cancellationToken);
        }

        private static ITestContainerBuilder GetTestContainerBuilder()
        {
            var builder = new TestContainerBuilder
            {
                AdditionalTypesRegistration = containerBuilder =>
                {
                    containerBuilder
                        .RegisterType<TDialog>()
                        .As<IDialog<object>>()
                        .InstancePerDependency();
                }
            };


            return builder;
        }
    }
}
