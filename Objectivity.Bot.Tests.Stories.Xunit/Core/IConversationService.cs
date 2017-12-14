namespace Objectivity.Bot.Tests.Stories.Xunit.Core
{
    using Microsoft.Bot.Connector;

    public interface IConversationService
    {
        string[] LatestOptions { get; set; }

        IMessageActivity GetToBotActivity(string text);
    }
}
