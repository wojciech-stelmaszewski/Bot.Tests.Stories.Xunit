namespace Objectivity.Bot.Tests.Stories.Xunit.Asserts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Core;
    using global::Xunit;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;
    using StoryModel;
    using StoryPerformer;
    using StoryPlayer;

    public static class StepAsserts
    {
        private const string NotEqualDialogStatusMessageFormat = "Expected dialog status = '{0}', actual status = '{1}'";
        private const string NotEqualDialogResultMessageFormat = "Dialog result = '{0}' doesn't match test predicate.";
        private const string WrongDialogResultTypeMessageFormat = "Dialog result = '{0}' is not of an expected type.";
        private const string ResultEmptyMessage = "Couldn't check result predicate - result is null.";
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

        public static void AssertDialogFinishStep(StoryStep storyStep, WrappedDialogResult dialogResult)
        {
            if (!(storyStep.StoryFrame is DialogStoryFrame dialogStoryFrame))
            {
                return;
            }

            var notEqualStatusesMessage = string.Format(
                    CultureInfo.CurrentCulture,
                    NotEqualDialogStatusMessageFormat,
                    dialogStoryFrame.DialogStatus,
                    dialogResult.DialogStatus);

            Assert.True(dialogStoryFrame.DialogStatus == dialogResult.DialogStatus, notEqualStatusesMessage);

            if (dialogStoryFrame.ResultPredicate != null && dialogResult.Result == null)
            {
                Assert.True(false, ResultEmptyMessage);
            }

            if (dialogStoryFrame.ResultPredicate != null)
            {
                try
                {
                    var notEqualResultMessage = string.Format(
                        CultureInfo.CurrentCulture,
                        NotEqualDialogResultMessageFormat,
                        dialogResult.Result);

                    Assert.True(dialogStoryFrame.ResultPredicate(dialogResult.Result), notEqualResultMessage);
                }
                catch (InvalidCastException)
                {
                    var wrongDialogResultTypeMessage = string.Format(
                        CultureInfo.CurrentCulture,
                        WrongDialogResultTypeMessageFormat,
                        dialogResult.Result);

                    Assert.True(false, wrongDialogResultTypeMessage);
                }
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
