namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPlayer
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NotMatchedUtteranceException : Exception
    {
        public NotMatchedUtteranceException()
        {
        }

        public NotMatchedUtteranceException(string message)
            : base(message)
        {
        }

        public NotMatchedUtteranceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NotMatchedUtteranceException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
