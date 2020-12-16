using System;
using Nuke.Common.Tooling;
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
            // TODO: Workaround
            var path = ToolPathResolver.GetPackageExecutable("JetBrains.ReSharper.GlobalTools", "dupfinder.exe");
            Environment.SetEnvironmentVariable("RESHARPER_EXE", path);

            ReSharperDupFinder(_ => _
                .SetSource(Config.Solution)
                .SetOutputFile(Config.XmlReportFile)
            );

            TransformXmlReportToHtmlReport();
        }
    }
}