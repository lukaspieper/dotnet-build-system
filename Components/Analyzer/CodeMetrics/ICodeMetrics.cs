using System.IO;
using Nuke.Common;
using Nuke.Common.Tools.CodeMetrics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using JetBrains.Annotations;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Components;
using static Nuke.Common.Assert;

namespace Components.Analyzer.CodeMetrics;

public interface ICodeMetrics : IAnalyzer<CodeMetricsUserConfig>, IHazBuildConfig, IHazSolution
{
    CodeMetricsUserConfig IAnalyzer<CodeMetricsUserConfig>.UserConfig => BuildConfig.CodeMetricsUserConfig;

    AbsolutePath IAnalyzer<CodeMetricsUserConfig>.XsltFile => XsltDirectory / "TransformCodeMetricsResults.xslt";

    [UsedImplicitly]
    Target CalculateMetrics => _ => _
        .DependsOn<ICopyStaticArtifacts>()
        .DependsOn<IRebuild>()
        .DependsOn(PrepareAnalyzer)
        .OnlyWhenStatic(() => UserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            // Without the environmental variable CodeMetrics will not provide any results
            var outputLines = DotNetTasks.DotNet("--info", logOutput: false);
            var basePathOutput = outputLines.Single(line => line.Text.Contains("Base Path:")).Text;
            var basePath = (AbsolutePath)basePathOutput.Substring(basePathOutput.IndexOf(':') + 1).Trim();
            
            Serilog.Log.Information($"SDK base path: {basePath}");
            EnvironmentInfo.SetVariable("MSBuildSDKsPath", basePath / "Sdks");

            CodeMetricsTasks.CodeMetrics(_ => _ 
                .SetSolution(Solution)
                .SetOutputFile(XmlReportFile)
            );
            
            TransformMetricsResults();
            FailTargetOnTooLowMaintainability();
        });
    
    private void TransformMetricsResults()
    {
        var arguments = new XsltArgumentList();
        arguments.AddParam("maintainability_index_minimum", "", UserConfig.MaintainabilityIndexMinimum);

        TransformXmlReportToHtmlReport(arguments);
    }

    private void FailTargetOnTooLowMaintainability()
    {
        var htmlOutput = File.ReadAllText(HtmlReportFile);

        var numberOfIssues = Regex.Matches(htmlOutput, "bgcolor=\"#FF221E\"").Count;
        if (numberOfIssues > 0)
        {
            Fail($"CodeMetrics analysis found {numberOfIssues} members with MaintainabilityIndex less than required value of '{UserConfig.MaintainabilityIndexMinimum}'.");
        }
    }
}