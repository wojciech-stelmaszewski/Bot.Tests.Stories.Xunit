namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPerformer.IO
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;
    using StoryModel;

    public interface IDialogWriter
    {
        Task SendActivity(IMessageActivity messageActivity);

        Task<IMessageActivity> GetStepMessageActivity(IStoryFrame frame);
    }
}
