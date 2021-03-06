﻿namespace Objectivity.Bot.Tests.Stories.Xunit.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using global::Xunit;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;
    using StoryModel;
    using StoryPerformer;
    using StoryPlayer;

    public static class StepAsserts
    {
        private const string MessageType = "message";

        public static void AssertStep(StoryStep storyStep, PerformanceStep performanceStep, string[] options = null)
        {
            switch (storyStep.Actor)
            {
                case Actor.Bot:
                    AssertBotStepMessage(storyStep, performanceStep);
                    break;
                case Actor.User:
                    AssertUserStepMessage(storyStep, performanceStep, options);
                    break;
            }
        }

        private static void AssertBotStepMessage(StoryStep storyStep, PerformanceStep performanceStep)
        {
            var message = performanceStep.MessageActivity;
            var frame = storyStep.StoryFrame;

            switch (frame.ComparisonType)
            {
                case ComparisonType.TextExact:
                    ProcessFrameTextExact(frame, performanceStep.MessageActivity.Type, performanceStep.Message);
                    break;

                case ComparisonType.TextMatchRegex:
                    ProcessBotFrameTextMatchRegex(frame, message);
                    break;

                case ComparisonType.AttachmentListPresent:
                    ProcessBotFrameListPresent(frame, message);
                    break;

                case ComparisonType.TextExactWithSuggestions:
                    ProcessBotFrameTextWithSuggestions(frame, message);
                    break;

                case ComparisonType.TextMatchRegexWithSuggestions:
                    ProcessBotFrameTextMatchRegexWithSuggestions(frame, message);
                    break;
                case ComparisonType.Predicate:
                    ProcessBotFramePredicate(frame, message);
                    break;
                default:
                    var reasonMessage = string.Format(
                        CultureInfo.InvariantCulture,
                        "Comparison type {0} is not supported for bot frame.",
                        frame.ComparisonType);

                    throw new ArgumentOutOfRangeException(nameof(frame.ComparisonType), reasonMessage);
            }
        }

        private static void AssertUserStepMessage(StoryStep storyStep, PerformanceStep performanceStep, string[] options = null)
        {
            var frame = storyStep.StoryFrame;

            switch (frame.ComparisonType)
            {
                case ComparisonType.TextExact:
                    ProcessFrameTextExact(frame, performanceStep.MessageActivity.Type, performanceStep.Message);
                    break;
                case ComparisonType.Option:
                    AssertUserFrameOption(frame, performanceStep.MessageActivity, options);
                    break;
                default:
                    var reasonMessage = string.Format(
                        CultureInfo.InvariantCulture,
                        "Comparison type {0} is not supported for user frame.",
                        frame.ComparisonType);

                    throw new ArgumentOutOfRangeException(nameof(frame.ComparisonType), reasonMessage);
            }
        }

        private static void AssertUserFrameOption(IStoryFrame storyFrame, IMessageActivity message, string[] options)
        {
            Assert.NotEmpty(options);

            var optionValue = options[storyFrame.OptionIndex];

            Assert.Equal(optionValue, message.Text);
        }

        private static void ProcessFrameTextExact(IStoryFrame storyFrame, string messageType, string message)
        {
            Assert.NotNull(message);
            Assert.Equal(MessageType, messageType);
            Assert.Equal(storyFrame.Text, message);
        }

        private static void ProcessBotFrameTextMatchRegex(IStoryFrame storyFrame, IMessageActivity message)
        {
            Assert.NotNull(message);
            Assert.Equal(MessageType, message.Type);
            Assert.Matches(storyFrame.Text, message.Text);
        }

        private static void ProcessBotFramePredicate(IStoryFrame storyFrame, IMessageActivity message)
        {
            Assert.True(storyFrame.MessageActivityPredicate(message));
        }

        private static void ProcessBotFrameTextMatchRegexWithSuggestions(IStoryFrame storyFrame, IMessageActivity message)
        {
            ProcessBotFrameTextMatchRegex(storyFrame, message);
            AssertSuggestions(storyFrame, message);
        }

        private static void ProcessBotFrameTextWithSuggestions(IStoryFrame storyFrame, IMessageActivity message)
        {
            ProcessFrameTextExact(storyFrame, message.Type, message.Text);
            AssertSuggestions(storyFrame, message);
        }

        private static void AssertSuggestions(IStoryFrame storyFrame, IMessageActivity message)
        {
            var botStoryFrame = storyFrame as BotStoryFrame;

            Assert.NotNull(botStoryFrame);
            Assert.Equal(botStoryFrame.Suggestions, message.SuggestedActions.Actions.Select(s => new KeyValuePair<string, object>(s.Title, s.Value)));
        }

        private static void ProcessBotFrameListPresent(IStoryFrame storyFrame, IMessageActivity message)
        {
            Assert.NotNull(message);
            Assert.Equal(1, message.Attachments.Count);
            Assert.IsType<JObject>(message.Attachments[0].Content);

            var listJson = (JObject)message.Attachments[0].Content;
            if (storyFrame.ListPredicate != null)
            {
                Assert.True(storyFrame.ListPredicate(listJson), "List contains expected item");
            }
        }
    }
}
