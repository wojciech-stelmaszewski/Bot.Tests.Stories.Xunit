namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPlayer
{
    using System;

    public class NotMatchedUtteranceException : Exception
    {
        public NotMatchedUtteranceException()
        {
        }

        public NotMatchedUtteranceException(string message)
            :base(message)
        {
        }


        public NotMatchedUtteranceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
