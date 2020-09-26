using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using Nuke.Common.Tooling;
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
            var metrics = ToolResolver.GetPackageTool("Microsoft.CodeAnalysis.Metrics", "Metrics.exe");
            metrics($"/SOLUTION:\"{Config.Solution}\" /OUT:\"{Config.XmlReportFile}\"");

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