using Nuke.Common.Tools.CodeMetrics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.ControlFlow;

namespace BuildSteps.CodeMetrics
{
    public class CodeMetricsStep : BuildStep<CodeMetricsUserConfig>
    {
        public CodeMetricsStep(BuildStepConfig<CodeMetricsUserConfig> config) : base(config)
        {
        }

        protected override void ExecuteStep()
        {
            // Without the environmental variable CodeMetrics will not provide any results
            var outputLines = DotNetTasks.DotNet("--info", logOutput: false);
            var basePathOutput = outputLines.Single(line => line.Text.Contains("Base Path:")).Text;
            var basePath = (AbsolutePath)basePathOutput.Substring(basePathOutput.IndexOf(':') + 1).Trim();
            
            Logger.Info($"SDK base path: {basePath}");
            EnvironmentInfo.SetVariable("MSBuildSDKsPath", basePath / "Sdks");

            CodeMetricsTasks.CodeMetrics(_ => _ 
                .SetSolution(Config.Solution)
                .SetOutputFile(Config.XmlReportFile)
            );

            TransformMetricsResults();
            FailTargetOnTooLowMaintainability();
        }

        private void TransformMetricsResults()
        {
            var arguments = new XsltArgumentList();
            arguments.AddParam("maintainability_index_minimum", "", UserConfig.MaintainabilityIndexMinimum);

            TransformXmlReportToHtmlReport(arguments);
        }

        private void FailTargetOnTooLowMaintainability()
        {
            var htmlOutput = File.ReadAllText(Config.HtmlReportFile);

            var numberOfIssues = Regex.Matches(htmlOutput, "bgcolor=\"#FF221E\"").Count;
            if (numberOfIssues > 0)
            {
                Fail($"CodeMetrics analysis found {numberOfIssues} members with MaintainabilityIndex less than required value of '{UserConfig.MaintainabilityIndexMinimum}'.");
            }
        }
    }
}