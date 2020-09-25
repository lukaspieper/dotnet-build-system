using System;
using System.IO;
using System.Text.RegularExpressions;
using Nuke.Common.Tools.DotCover;
using static Nuke.Common.ControlFlow;
using static Nuke.Common.Logger;

namespace BuildSteps.DotCover
{
    public class DotCoverStep : BuildStep<DotCoverUserConfig>
    {
        private readonly bool RunRestore;
        private readonly bool RunBuild;

        public DotCoverStep(BuildStepConfig<DotCoverUserConfig> config, bool runRestore, bool runBuild) : base(config)
        {
            RunRestore = runRestore;
            RunBuild = runBuild;
        }

        protected override void ExecuteStep()
        {
            try
            {
                var arguments = BuildDotCoverArguments();
                DotCoverTasks.DotCover(arguments, Config.BuildStepArtifactsDirectory);
            }
            catch (Exception e)
            {
                Warn(e);
            }

            TransformXmlReportToHtmlReport();
            FailTargetOnFailingTest();
        }

        private string BuildDotCoverArguments()
        {
            var arguments =
                $"dotnet --output=DotCover.html --reportType=HTML --Filters={UserConfig.DotCoverCoverageFilter} -- test {Config.Solution} --logger \"trx;LogFileName={Config.XmlReportFile}\"";

            if (RunRestore)
            {
                arguments += " --no-restore";
            }

            if (RunBuild)
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