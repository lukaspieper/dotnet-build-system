using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.ReSharper;
using Nuke.Components;
using static Nuke.Common.Tools.ReSharper.ReSharperTasks;

namespace Components.Analyzer.ReSharper;

public interface IReSharperInspection : IAnalyzer<ReSharperInspectionUserConfig>, IHazBuildConfig, IHazSolution
{
    ReSharperInspectionUserConfig IAnalyzer<ReSharperInspectionUserConfig>.UserConfig => BuildConfig.ReSharperInspectionUserConfig;

    AbsolutePath IAnalyzer<ReSharperInspectionUserConfig>.XsltFile => XsltDirectory / "TransformCodeInspectionResults.xslt";

    [UsedImplicitly]
    Target RunReSharperInspection => _ => _
        .DependsOn<ICopyStaticArtifacts>()
        .DependsOn<IRebuild>()
        .OnlyWhenStatic(() => UserConfig.Enabled)
        .Executes(() =>
        {
            CleanAnalyzerDirectories();
            
            ReSharperInspectCode(_ => _
                .SetTargetPath(Solution)
                .SetOutput(XmlReportFile)
                .SetCachesHome(AnalyzerCacheDirectory)
            );

            TransformXmlReportToHtmlReport();
            LogCodeInspectionResults();
        });
    
    private void LogCodeInspectionResults()
    {
        var htmlOutput = File.ReadAllText(HtmlReportFile);

        var numberOfWarnings = Regex.Matches(htmlOutput, "WARNING").Count;
        if (numberOfWarnings > 0)
        {
            Serilog.Log.Warning($"ReSharper Code Inspection found {numberOfWarnings} warnings.");
        }

        var numberOfSuggestions = Regex.Matches(htmlOutput, "SUGGESTION").Count;
        if (numberOfSuggestions > 0)
        {
            Serilog.Log.Warning($"ReSharper Code Inspection found {numberOfSuggestions} suggestions.");
        }
    }
}