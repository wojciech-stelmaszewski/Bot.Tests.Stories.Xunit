namespace Objectivity.Bot.Tests.Stories.Xunit.Core
{
    using System;

    [Serializable]
    public class WrappedDialogResult
    {
        public WrappedDialogResult()
        {
            this.DialogStatus = DialogStatus.Busy;
        }

        public object Result { get; set; }

        public DialogStatus DialogStatus { get; set; }
    }
}
