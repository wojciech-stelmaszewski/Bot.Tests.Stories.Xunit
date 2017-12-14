namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPerformer
{
    using System.Linq;
    using Core;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;
    using StoryModel;

    public class PerformanceStep : IStep
    {
        private const string OptionsToken = "buttons";
        private const string TokenValueKey = "value";

        public PerformanceStep()
        {
        }

        public PerformanceStep(IMessageActivity messageActivity)
        {
            this.MessageActivity = messageActivity;
            this.Message = messageActivity.Text;

            this.TrySetOptions();
        }

        public IMessageActivity MessageActivity { get; set; }

        public Actor Actor { get; set; }

        public int StepIndex { get; set; }

        public string Message { get; set; }

        public string[] Options { get; set; }

        private void TrySetOptions()
        {
            if (this.MessageActivity.Attachments == null || this.MessageActivity.Attachments.Count != 1)
            {
                return;
            }

            var listJson = (JObject)this.MessageActivity.Attachments[0].Content;
            var token = listJson.SelectToken(OptionsToken);

            if (token == null)
            {
                return;
            }

            this.Options = token.Select(item => item[TokenValueKey].ToString()).ToArray();
        }
    }
}
