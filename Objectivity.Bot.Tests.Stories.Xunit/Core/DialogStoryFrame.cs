namespace Objectivity.Bot.Tests.Stories.Xunit.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;
    using Objectivity.Bot.Tests.Stories.StoryModel;
    using static Microsoft.Bot.Builder.Luis.Models.DialogResponse;

    public class DialogStoryFrame : IStoryFrame
    {
        public DialogStoryFrame(DialogStatus dialogStatus, Predicate<object> resultPredicate = null)
        {
            this.DialogStatus = dialogStatus;
            this.ResultPredicate = resultPredicate;
        }

        public DialogStatus DialogStatus { get; }

        public Predicate<object> ResultPredicate { get; }

        public Actor Actor => Actor.Bot;

        public ComparisonType ComparisonType => ComparisonType.Predicate;

        public Predicate<JObject> ListPredicate { get; }

        public Predicate<IMessageActivity> MessageActivityPredicate { get; }

        public int OptionIndex { get; }

        public string OptionOutputPlaceholder { get; }

        public string Text { get; }
    }
}
