namespace Objectivity.Bot.Tests.Stories.Xunit
{
    using System;
    using Core;
    using Recorder;
    using StoryModel;

    public static class StoryRecorderExtensions
    {
        public static IStory DialogDone(this IStoryRecorder recorder)
        {
            return GetResultStory(recorder, DialogStatus.Finished);
        }

        public static IStory DialogDoneWithResult(this IStoryRecorder recorder, Predicate<object> resultPredicate)
        {
            return GetResultStory(recorder, DialogStatus.Finished, resultPredicate);
        }

        public static IStory DialogDoneWithResult<T>(this IStoryRecorder recorder, Predicate<T> resultPredicate)
        {
            return GetResultStory(recorder, DialogStatus.Finished, resultObject =>
            {
                T result;

                try
                {
                    result = (T)resultObject;
                }
                catch
                {
                    var message =
                        $"Unable to cast result of type '{resultObject.GetType().Name}' to type '{typeof(T).Name}'.";

                    throw new FormatException(message);
                }

                return resultPredicate(result);
            });
        }

        public static IStory DialogFailed(this IStoryRecorder recorder)
        {
            return GetResultStory(recorder, DialogStatus.Failed);
        }

        public static IStory DialogFailedWithExceptionOfType<TExceptionType>(this IStoryRecorder recorder)
        {
            return GetResultStory(recorder, DialogStatus.Failed, null, typeof(TExceptionType));
        }

        private static IStory GetResultStory(IStoryRecorder storyRecorder, DialogStatus resultType, Predicate<object> resultPredicate = null, Type exceptionType = null)
        {
            var story = storyRecorder.Rewind();

            story.AddStoryFrame(new DialogStoryFrame(resultType, resultPredicate,  exceptionType));

            return story;
        }
    }
}
