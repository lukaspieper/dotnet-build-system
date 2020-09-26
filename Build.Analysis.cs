using System.Collections.Generic;
using BuildSteps;
using BuildSteps.CodeMetrics;
using BuildSteps.DotCover;
using BuildSteps.JetBrainsDupFinder;
using BuildSteps.ReSharperInspection;
using BuildSteps.RoslynAnalyzers;
using Nuke.Common;
using Nuke.Common.CI;
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
            var xsltFile = XsltDirectory / "TransformRoslynAnalyzersResults.xslt";
            var stepConfig = new BuildStepConfig<RoslynAnalyzersUserConfig>(BuildConfig.RoslynAnalyzersUserConfig, Solution, ArtifactsDirectory, xsltFile);
            new RoslynAnalyzersStep(stepConfig, MsBuildOutput).Execute();
        });

    private Target CalculateMetrics => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.CodeMetricsUserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var xsltFile = XsltDirectory / "TransformCodeMetricsResults.xslt";
            var stepConfig = new BuildStepConfig<CodeMetricsUserConfig>(BuildConfig.CodeMetricsUserConfig, Solution, ArtifactsDirectory, xsltFile);
            new CodeMetricsStep(stepConfig).Execute();
        });

    private Target RunReSharperInspection => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.ReSharperInspectionUserConfig.Enabled)
        .Executes(() =>
        {
            var xsltFile = XsltDirectory / "TransformCodeInspectionResults.xslt";
            var stepConfig = new BuildStepConfig<ReSharperInspectionUserConfig>(BuildConfig.ReSharperInspectionUserConfig, Solution, ArtifactsDirectory, xsltFile);
            new ReSharperInspectionStep(stepConfig).Execute();
        });

    private Target FindDuplications => _ => _
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.JetBrainsDupFinderUserConfig.Enabled)
        .Executes(() =>
        {
            var xsltFile = XsltDirectory / "TransformDupFinderResults.xslt";
            var stepConfig = new BuildStepConfig<JetBrainsDupFinderUserConfig>(BuildConfig.JetBrainsDupFinderUserConfig, Solution, ArtifactsDirectory, xsltFile);
            new JetBrainsDupFinderStep(stepConfig).Execute();
        });

    private Target RunTestsWithCoverage => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .OnlyWhenStatic(() => BuildConfig.DotCoverUserConfig.Enabled)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var xsltFile = XsltDirectory / "TransformTrx.xslt";
            var stepConfig = new BuildStepConfig<DotCoverUserConfig>(BuildConfig.DotCoverUserConfig, Solution, ArtifactsDirectory, xsltFile);
            new DotCoverStep(stepConfig, InvokedTargets.Contains(Restore), InvokedTargets.Contains(Compile)).Execute();
        });
}