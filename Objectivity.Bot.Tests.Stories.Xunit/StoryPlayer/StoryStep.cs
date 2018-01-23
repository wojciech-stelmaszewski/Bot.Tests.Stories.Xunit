namespace Objectivity.Bot.Tests.Stories.Xunit.StoryPlayer
{
    using Core;
    using Player;
    using StoryModel;

    public class StoryStep : IStep
    {
        public StoryStep()
        {
        }

        public StoryStep(IStoryFrame storyFrame)
        {
            this.StoryFrame = storyFrame;
            this.Message = storyFrame.Text;
            this.Actor = storyFrame.Actor;
        }

        public int StepIndex { get; set; }

        public string Message { get; set; }

        public bool IsDialogResultCheckupStep => this.StoryFrame != null && this.StoryFrame is DialogStoryFrame;

        public StoryPlayerStepStatus Status { get; set; }

        public IStoryFrame StoryFrame { get; set; }

        public Actor Actor { get; set; }
    }
}
