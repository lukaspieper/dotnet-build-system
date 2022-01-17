using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.ReSharper;
using Nuke.Components;
using static Nuke.Common.Tools.ReSharper.ReSharperTasks;

namespace Components.Analyzer.DupFinder;

public interface IDupFinder : IAnalyzer<JetBrainsDupFinderUserConfig>, IHazBuildConfig, IHazSolution
{
    JetBrainsDupFinderUserConfig IAnalyzer<JetBrainsDupFinderUserConfig>.UserConfig => BuildConfig.JetBrainsDupFinderUserConfig;

    AbsolutePath IAnalyzer<JetBrainsDupFinderUserConfig>.XsltFile => XsltDirectory / "TransformDupFinderResults.xslt";

    [UsedImplicitly]
    Target FindDuplications => _ => _
        .DependsOn<ICopyStaticArtifacts>()
        .DependsOn<IRebuild>()
        .OnlyWhenStatic(() => BuildConfig.JetBrainsDupFinderUserConfig.Enabled)
        .Executes(() =>
        {
            CleanAnalyzerDirectories();
            
            ReSharperDupFinder(_ => _
                .SetSource(Solution)
                .SetOutputFile(XmlReportFile)
            );

            TransformXmlReportToHtmlReport();
        });
}