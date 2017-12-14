namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPlayer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Asserts;
    using Autofac;
    using Container;
    using Core;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Player;
    using StoryModel;
    using StoryPerformer;

    public class UnitTestStoryPlayer : IStoryPlayer
    {
        private readonly ITestContainerBuilder testContainerBuilder;

        public UnitTestStoryPlayer(ITestContainerBuilder testContainerBuilder)
        {
            this.testContainerBuilder = testContainerBuilder;
        }

        public async Task<IStoryResult> Play(IStory story, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var container = this.GetDialogTestContainer())
            using (var scopeContext = this.InitializeScopeContext(container))
            {
                var testDialog = container.Resolve<IDialog<object>>();
                var storyPerformer = container.Resolve<IStoryPerformer>();

                DialogModule_MakeRoot.Register(scopeContext.Scope, () => testDialog);

                var performanceSteps = await storyPerformer.Perform(story);

                await StoryAsserts.AssertStory(story, performanceSteps);
            }

            return new StoryResult();
        }

        private IContainer GetDialogTestContainer()
        {
            using (new ResolveMoqAssembly())
            {
                var options = TestContainerBuilderOptions.MockConnectorFactory
                    | TestContainerBuilderOptions.ScopedQueue
                    | TestContainerBuilderOptions.ResolveDialogFromContainer;

                return this.testContainerBuilder.Build(options);
            }
        }

        private IScopeContext InitializeScopeContext(IContainer container)
        {
            var scopeContext = new ScopeContext(container);
            var builder = new ContainerBuilder();
            var module = new UnitTestStoryPlayerModule(scopeContext);

            builder.RegisterModule(module);
            builder.Update(container);

            return scopeContext;
        }
    }
}
