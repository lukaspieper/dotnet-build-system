using Nuke.Common.Tools.ReSharper;
using static Nuke.Common.Tools.ReSharper.ReSharperTasks;

namespace BuildSteps.JetBrainsDupFinder
{
    public class JetBrainsDupFinderStep : BuildStep<JetBrainsDupFinderUserConfig>
    {
        public JetBrainsDupFinderStep(BuildStepConfig<JetBrainsDupFinderUserConfig> config) : base(config)
        {
        }

        protected override void ExecuteStep()
        {
            ReSharperDupFinder(_ => _
                .SetSource(Config.Solution)
                .SetOutputFile(Config.XmlReportFile)
            );

            TransformXmlReportToHtmlReport();
        }
    }
}