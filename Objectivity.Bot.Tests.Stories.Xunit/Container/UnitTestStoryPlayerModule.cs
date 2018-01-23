namespace Objectivity.Bot.Tests.Stories.Xunit.Container
{
    using Autofac;
    using Core;
    using Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Objectivity.Bot.Tests.Stories.Xunit.Asserts;
    using StoryPerformer;

    public class UnitTestStoryPlayerModule : Module
    {
        private readonly IScopeContext scopeContext;

        public UnitTestStoryPlayerModule(IScopeContext scopeContext)
        {
            this.scopeContext = scopeContext;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<WrappedDialogResult>();
            builder.RegisterType<StoryAsserts>();

            builder
                .Register<IDialog<object>>(ctx =>
                    new WrapperDialog(
                        ctx.ResolveKeyed<IDialog<object>>(Consts.TargetDialogKey),
                        ctx.Resolve<WrappedDialogResult>()))
                .As<IDialog<object>>()
                .InstancePerDependency();

            builder
                .Register(c => this.scopeContext)
                .As<IScopeContext>()
                .SingleInstance();

            builder
                .RegisterType<WrappedStoryPerformer>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
