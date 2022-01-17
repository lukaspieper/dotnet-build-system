using System;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotCover;
using static Nuke.Common.Assert;
using static Nuke.Common.Tools.DotCover.DotCoverTasks;

namespace Components.Analyzer.DotCover;

public interface IDotCover : IAnalyzer<DotCoverUserConfig>, IHazBuildConfig, IRebuild
{
    DotCoverUserConfig IAnalyzer<DotCoverUserConfig>.UserConfig => BuildConfig.DotCoverUserConfig;

    AbsolutePath IAnalyzer<DotCoverUserConfig>.XsltFile => XsltDirectory / "TransformTrx.xslt";

    [UsedImplicitly]
    Target RunTestsWithCoverage => _ => _
        .DependsOn<ICopyStaticArtifacts>()
        .DependsOn<IRebuild>()
        .OnlyWhenStatic(() => UserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            CleanAnalyzerDirectories();

            try
            {
                DotCoverCoverDotNet(_ => _
                    .SetTargetArguments(BuildDotNetTestArguments())
                    .SetReportType(DotCoverReportType.Html)
                    .SetFilters(UserConfig.DotCoverCoverageFilter)
                    .SetOutputFile("DotCover.html")
                    .SetProcessWorkingDirectory(AnalyzerReportDirectory)
                );
            }
            catch (Exception e)
            {
                Serilog.Log.Warning(e.ToString());
            }

            TransformXmlReportToHtmlReport();
            FailTargetOnFailingTest();
        });

    private string BuildDotNetTestArguments()
    {
        var arguments = $"test {Solution} --logger \"trx;LogFileName={XmlReportFile}\"";

        if (SucceededTargets.Contains(Restore))
        {
            arguments += " --no-restore";
        }

        if (SucceededTargets.Contains(Compile))
        {
            arguments += " --no-build";
        }

        return arguments;
    }

    private void FailTargetOnFailingTest()
    {
        var htmlOutput = File.ReadAllText(HtmlReportFile);

        var numberOfFailingTests = Regex.Matches(htmlOutput, "alt=\"Failed\"").Count;
        if (numberOfFailingTests > 0)
        {
            Fail($"{numberOfFailingTests} tests failed.");
        }
    }
}