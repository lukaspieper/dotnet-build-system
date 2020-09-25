using Nuke.Common.Tools.DupFinder;
using static Nuke.Common.Tools.DupFinder.DupFinderTasks;

namespace BuildSteps.JetBrainsDupFinder
{
    public class JetBrainsDupFinderStep : BuildStep<JetBrainsDupFinderUserConfig>
    {
        public JetBrainsDupFinderStep(BuildStepConfig<JetBrainsDupFinderUserConfig> config) : base(config)
        {
        }

        protected override void ExecuteStep()
        {
            DupFinder(_ => _
                .SetSource(Config.Solution)
                .SetOutputFile(Config.XmlReportFile)
            );

            TransformXmlReportToHtmlReport();
        }
    }
}