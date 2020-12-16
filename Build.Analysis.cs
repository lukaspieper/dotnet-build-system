using System.Collections.Generic;
using BuildSteps;
using BuildSteps.CodeMetrics;
using BuildSteps.DotCover;
using BuildSteps.JetBrainsDupFinder;
using BuildSteps.ReSharperInspection;
using BuildSteps.RoslynAnalyzers;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

public partial class Build
{
    IReadOnlyCollection<Output> MsBuildOutput;

    private Target CompileAndAnalyze => _ => _
        .Produces(ArtifactsDirectory)
        .DependsOn(Compile)
        .DependsOn(GetRoslynAnalyzersResults)
        .DependsOn(CalculateMetrics)
        .DependsOn(RunReSharperInspection)
        .DependsOn(FindDuplications)
        .DependsOn(RunTestsWithCoverage);

    private Target GetRoslynAnalyzersResults => _ => _
        .DependsOn(Compile)
        .DependsOn(CopyStaticArtifacts)
        .OnlyWhenStatic(() => BuildConfig.RoslynAnalyzersUserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.RoslynAnalyzersUserConfig, XsltDirectory / "TransformRoslynAnalyzersResults.xslt");
            new RoslynAnalyzersStep(stepConfig, MsBuildOutput).Execute();
        });

    private Target CalculateMetrics => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.CodeMetricsUserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.CodeMetricsUserConfig, XsltDirectory / "TransformCodeMetricsResults.xslt");
            new CodeMetricsStep(stepConfig).Execute();
        });

    private Target RunReSharperInspection => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.ReSharperInspectionUserConfig.Enabled)
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.ReSharperInspectionUserConfig, XsltDirectory / "TransformCodeInspectionResults.xslt");
            new ReSharperInspectionStep(stepConfig).Execute();
        });

    private Target FindDuplications => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.JetBrainsDupFinderUserConfig.Enabled)
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.JetBrainsDupFinderUserConfig, XsltDirectory / "TransformDupFinderResults.xslt");
            new JetBrainsDupFinderStep(stepConfig).Execute();
        });

    private Target RunTestsWithCoverage => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.DotCoverUserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var stepConfig = CreateBuildStepConfig(BuildConfig.DotCoverUserConfig, XsltDirectory / "TransformTrx.xslt");
            new DotCoverStep(stepConfig, InvokedTargets.Contains(Restore), InvokedTargets.Contains(Compile)).Execute();
        });

    private BuildStepConfig<T> CreateBuildStepConfig<T>(T userConfig, AbsolutePath xsltFile = null) where T : IBuildStepUserConfig
    {
        return new BuildStepConfig<T>(userConfig, Solution, ArtifactsDirectory, CacheDirectory, xsltFile);
    }
}