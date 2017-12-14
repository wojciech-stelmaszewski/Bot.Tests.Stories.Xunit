namespace Objectivity.Bot.Tests.Stories.Xunit.Core
{
    using StoryModel;

    public interface IStep
    {
        Actor Actor { get; set; }

        int StepIndex { get; set; }

        string Message { get; set; }
    }
}
