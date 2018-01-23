﻿namespace Objectivity.Bot.Tests.Stories.Xunit
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

        public static IStory DialogFailed(this IStoryRecorder recorder)
        {
            return GetResultStory(recorder, DialogStatus.Failed);
        }

        private static IStory GetResultStory(IStoryRecorder storyRecorder, DialogStatus resultType, Predicate<object> resultPredicate = null)
        {
            var story = storyRecorder.Rewind();

            story.AddStoryFrame(new DialogStoryFrame(resultType, resultPredicate));

            return story;
        }
    }
}
