using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using static Nuke.Common.Assert;

namespace Components.Analyzer.RoslynAnalyzers;

public interface IRoslynAnalyzers : IAnalyzer<RoslynAnalyzersUserConfig>, IHazBuildConfig, IRebuild
{
    RoslynAnalyzersUserConfig IAnalyzer<RoslynAnalyzersUserConfig>.UserConfig => BuildConfig.RoslynAnalyzersUserConfig;

    AbsolutePath IAnalyzer<RoslynAnalyzersUserConfig>.XsltFile => XsltDirectory / "TransformRoslynAnalyzersResults.xslt";

    [UsedImplicitly]
    Target GetRoslynAnalyzersResults => _ => _
        .DependsOn<IRebuild>()
        .DependsOn<ICopyStaticArtifacts>()
        .OnlyWhenStatic(() => UserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            CleanAnalyzerDirectories();
            var analyzerResults = GetRoslynAnalyzersResultsFromBuildOutput();

            WriteResultsToXmlFile(analyzerResults);
            TransformXmlReportToHtmlReport();

            if (analyzerResults.Count > UserConfig.RoslynAnalyzersWarningThreshold)
            {
                Fail($"RoslynAnalyzers found too many warnings ({analyzerResults.Count}, max. {UserConfig.RoslynAnalyzersWarningThreshold}).");
            }
            else if (analyzerResults.Count > 0)
            {
                Serilog.Log.Warning($"RoslynAnalyzers found {analyzerResults.Count} warnings.");
            }
        });

    private List<AnalyzerResult> GetRoslynAnalyzersResultsFromBuildOutput()
    {
        var analyzerResultRegex = new Regex(@"^\s*(?'Location'\S*[\(\d*,\d*\)]?)\s*:\s*(?'Severity'[a-z]*)\s*(?'Code'[A-Z]{2}\d{4}):\s*(?'Description'.*)\s\[(?'Project'.*)\]");

        return MsBuildOutput.Select(o => o.Text.Trim())
            .Distinct()
            .Select(line => analyzerResultRegex.Match(line))
            .Where(match => match.Success)
            .Select(match => new AnalyzerResult(match))
            .OrderBy(result => result.Location)
            .ToList();
    }

    private void WriteResultsToXmlFile(List<AnalyzerResult> results)
    {
        var serializer = new XmlSerializer(typeof(List<AnalyzerResult>), new XmlRootAttribute("AnalyzerResults"));

        using var writer = new StreamWriter(XmlReportFile);
        serializer.Serialize(writer, results);
    }
}