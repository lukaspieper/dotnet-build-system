using BuildSteps;
using Components;
using Components.Analyzer.CodeMetrics;
using Components.Analyzer.RoslynAnalyzers;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Components;

public partial class Build : ICodeMetrics, IRoslynAnalyzers
{
    private Target CompileAndAnalyze => _ => _
        .Produces((this as IHazArtifacts).ArtifactsDirectory)
        .DependsOn<IRebuild>()
        .DependsOn<IRoslynAnalyzers>()
        .DependsOn<ICodeMetrics>();
        //.DependsOn(RunReSharperInspection)
        //.DependsOn(FindDuplications)
        //.DependsOn(RunTestsWithCoverage);

    /*    private Target RunReSharperInspection => _ => _
        .DependsOn<ICopyStaticArtifacts>()
        .DependsOn<IRebuild>()
        .OnlyWhenStatic(() => BuildConfig.ReSharperInspectionUserConfig.Enabled)
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.ReSharperInspectionUserConfig, XsltDirectory / "TransformCodeInspectionResults.xslt");
            new ReSharperInspectionStep(stepConfig).Execute();
        });

    private Target FindDuplications => _ => _
        .DependsOn<ICopyStaticArtifacts>()
        .DependsOn<IRebuild>()
        .OnlyWhenStatic(() => BuildConfig.JetBrainsDupFinderUserConfig.Enabled)
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.JetBrainsDupFinderUserConfig, XsltDirectory / "TransformDupFinderResults.xslt");
            new JetBrainsDupFinderStep(stepConfig).Execute();
        });

    private Target RunTestsWithCoverage => _ => _
        .DependsOn<ICopyStaticArtifacts>()
        .DependsOn<IRebuild>()
        .OnlyWhenStatic(() => BuildConfig.DotCoverUserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.DotCoverUserConfig, XsltDirectory / "TransformTrx.xslt");
            new DotCoverStep(stepConfig, InvokedTargets.Contains((this as IRestore).Restore), InvokedTargets.Contains((this as IRebuild).Compile)).Execute();
        });*/

    private BuildStepConfig<T> CreateBuildStepConfig<T>(T userConfig, AbsolutePath xsltFile = null) where T : IBuildStepUserConfig
    {
        return new BuildStepConfig<T>(userConfig, (this as IHazSolution).Solution, (this as IHazArtifacts).ArtifactsDirectory, (this as IHazCacheDirectory).CacheDirectory, xsltFile);
    }
}