namespace Objectivity.Bot.Tests.Stories.Xunit.Asserts
{
    using System;
    using System.Globalization;
    using Core;
    using global::Xunit;
    using StoryPlayer;

    public static class FinishStepAsserts
    {
        private const string WrongExceptionTypeMessageFormat = "Expected dialog fail with exception of type = '{0}', actual exception type = '{1}'";
        private const string NotEqualDialogStatusMessageFormat = "Expected dialog status = '{0}', actual status = '{1}'";
        private const string NotEqualDialogResultMessageFormat = "Dialog result = '{0}' doesn't match test predicate.";
        private const string WrongDialogResultTypeMessageFormat = "Dialog result = '{0}' is not of an expected type.";
        private const string ResultEmptyMessage = "Couldn't check result predicate - result is null.";

        public static void AssertDialogFinishStep(StoryStep storyStep, WrappedDialogResult dialogResult)
        {
            if (!(storyStep.StoryFrame is DialogStoryFrame dialogStoryFrame))
            {
                return;
            }

            VerifyStatusesEqual(dialogResult, dialogStoryFrame);
            VerifyResultNotEmpty(dialogResult, dialogStoryFrame);

            if (dialogStoryFrame.ResultPredicate != null)
            {
                VerifyResultPredicate(dialogResult, dialogStoryFrame);
            }

            if (dialogStoryFrame.ExceptionType != null)
            {
                VerifyExceptionType(dialogResult, dialogStoryFrame);
            }
        }

        private static void VerifyExceptionType(WrappedDialogResult dialogResult, DialogStoryFrame dialogStoryFrame)
        {
            var exceptionType = dialogResult.Exception.GetType();

            var wrongExceptionTypeMessage = string.Format(
                CultureInfo.CurrentCulture,
                WrongExceptionTypeMessageFormat,
                dialogStoryFrame.ExceptionType.Name,
                exceptionType.Name);

            Assert.True(
                dialogResult.Exception.GetType() == dialogStoryFrame.ExceptionType,
                wrongExceptionTypeMessage);
        }

        private static void VerifyResultPredicate(WrappedDialogResult dialogResult, DialogStoryFrame dialogStoryFrame)
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

        private static void VerifyResultNotEmpty(WrappedDialogResult dialogResult, DialogStoryFrame dialogStoryFrame)
        {
            if (dialogStoryFrame.ResultPredicate != null && dialogResult.Result == null)
            {
                Assert.True(false, ResultEmptyMessage);
            }
        }

        private static void VerifyStatusesEqual(WrappedDialogResult dialogResult, DialogStoryFrame dialogStoryFrame)
        {
            var notEqualStatusesMessage = string.Format(
                            CultureInfo.CurrentCulture,
                            NotEqualDialogStatusMessageFormat,
                            dialogStoryFrame.DialogStatus,
                            dialogResult.DialogStatus);

            Assert.True(dialogStoryFrame.DialogStatus == dialogResult.DialogStatus, notEqualStatusesMessage);
        }
    }
}
