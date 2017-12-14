namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPerformer.IO
{
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;

    public interface IDialogReader
    {
        List<IMessageActivity> GetMessageActivities();

        List<IMessageActivity> DequeueStartupMessageActivities();
    }
}
