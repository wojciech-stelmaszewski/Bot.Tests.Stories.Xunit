namespace Objectivity.Bot.Tests.Stories.Xunit.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using StoryPerformer;

    public static class PerformanceStepListExtensions
    {
        public static string[] TryGetOptions(this List<PerformanceStep> performanceSteps, int takeCount = 0)
        {
            takeCount = Math.Min(takeCount <= 0 ? performanceSteps.Count : takeCount, performanceSteps.Count);

            var takenSteps = performanceSteps.Take(takeCount).ToList();
            var performanceStep = takenSteps.LastOrDefault(step => step.Options != null);

            return performanceStep?.Options;
        }

        public static void AddNotNullStep(this List<PerformanceStep> performanceSteps, PerformanceStep performanceStep)
        {
            if (performanceStep != null)
            {
                performanceStep.StepIndex = performanceSteps.Count;

                performanceSteps.Add(performanceStep);
            }
        }
    }
}
