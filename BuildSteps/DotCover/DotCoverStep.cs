using System;
using System.IO;
using System.Text.RegularExpressions;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotCover;
using static Nuke.Common.ControlFlow;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.DotCover.DotCoverTasks;

namespace BuildSteps.DotCover
{
    public class DotCoverStep : BuildStep<DotCoverUserConfig>
    {
        private readonly bool SkipRestore;
        private readonly bool SkipBuild;

        public DotCoverStep(BuildStepConfig<DotCoverUserConfig> config, bool skipRestore, bool skipBuild) : base(config)
        {
            SkipRestore = skipRestore;
            SkipBuild = skipBuild;
        }

        protected override void ExecuteStep()
        {
            try
            {
                DotCoverCoverDotNet(_ => _
                    .SetTargetArguments(BuildDotNetTestArguments())
                    .SetReportType(DotCoverReportType.Html)
                    .SetFilters(UserConfig.DotCoverCoverageFilter)
                    .SetOutputFile("DotCover.html")
                    .SetProcessWorkingDirectory(Config.BuildStepArtifactsDirectory)
                );
            }
            catch (Exception e)
            {
                Warn(e);
            }

            TransformXmlReportToHtmlReport();
            FailTargetOnFailingTest();
        }

        private string BuildDotNetTestArguments()
        {
            var arguments = $"test {Config.Solution} --logger \"trx;LogFileName={Config.XmlReportFile}\"";

            if (SkipRestore)
            {
                arguments += " --no-restore";
            }

            if (SkipBuild)
            {
                arguments += " --no-build";
            }

            return arguments;
        }

        private void FailTargetOnFailingTest()
        {
            var htmlOutput = File.ReadAllText(Config.HtmlReportFile);

            var numberOfFailingTests = Regex.Matches(htmlOutput, "alt=\"Failed\"").Count;
            if (numberOfFailingTests > 0)
            {
                Fail($"{numberOfFailingTests} tests failed.");
            }
        }
    }
}