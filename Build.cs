using System.Collections.Generic;
using System.Diagnostics;
using BuildSteps;
using BuildSteps.CodeMetrics;
using BuildSteps.DotCover;
using BuildSteps.JetBrainsDupFinder;
using BuildSteps.ReSharperInspection;
using BuildSteps.RoslynAnalyzers;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.CompileAndAnalyze);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] Solution Solution { get; }

    AbsolutePath BuildDirectory => RootDirectory / "build";
    AbsolutePath XsltDirectory => BuildDirectory / "Xslt";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath AnalysisArtifactsDirectory => ArtifactsDirectory / "Analysis";

    IReadOnlyCollection<Output> MsBuildOutput;

    [UsedImplicitly]
    Target OpenReport => _ => _
        .Executes(() =>
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = ArtifactsDirectory / "Analysis.html",
                UseShellExecute = true,
            };

            Process.Start(processStartInfo);
        });

    Target CopyStaticArtifacts => _ => _
        .After(Clean)
        .Executes(() =>
        {
            EnsureExistingDirectory(ArtifactsDirectory);

            CopyDirectoryRecursively(BuildDirectory / "StaticArtifacts",
                ArtifactsDirectory,
                DirectoryExistsPolicy.Merge,
                FileExistsPolicy.OverwriteIfNewer);
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj", "**/AppPackages").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            MsBuildOutput = MSBuild(s => s
                .SetTargetPath(Solution)
                .SetConfiguration(Configuration));
        });

    Target GetRoslynAnalyzersResults => _ => _
        .DependsOn(Compile)
        .DependsOn(CopyStaticArtifacts)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var userConfig = new RoslynAnalyzersUserConfig();
            var xsltFile = XsltDirectory / "TransformRoslynAnalyzersResults.xslt";
            var stepConfig = new BuildStepConfig<RoslynAnalyzersUserConfig>(userConfig, Solution, ArtifactsDirectory, xsltFile);
            new RoslynAnalyzersStep(stepConfig, MsBuildOutput).Execute();
        });

    Target CalculateMetrics => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var userConfig = new CodeMetricsUserConfig();
            var xsltFile = XsltDirectory / "TransformCodeMetricsResults.xslt";
            var stepConfig = new BuildStepConfig<CodeMetricsUserConfig>(userConfig, Solution, ArtifactsDirectory, xsltFile);
            new CodeMetricsStep(stepConfig).Execute();
        });

    Target RunReSharperInspection => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .Executes(() =>
        {
            var userConfig = new ReSharperInspectionUserConfig();
            var xsltFile = XsltDirectory / "TransformCodeInspectionResults.xslt";
            var stepConfig = new BuildStepConfig<ReSharperInspectionUserConfig>(userConfig, Solution, ArtifactsDirectory, xsltFile);
            new ReSharperInspectionStep(stepConfig).Execute();
        });

    Target FindDuplications => _ => _
        .After(Compile)
        .Executes(() =>
        {
            var userConfig = new JetBrainsDupFinderUserConfig();
            var xsltFile = XsltDirectory / "TransformDupFinderResults.xslt";
            var stepConfig = new BuildStepConfig<JetBrainsDupFinderUserConfig>(userConfig, Solution, ArtifactsDirectory, xsltFile);
            new JetBrainsDupFinderStep(stepConfig).Execute();
        });

    Target RunTestsWithCoverage => _ => _
        .DependsOn(CopyStaticArtifacts)
        .After(Compile)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var userConfig = new DotCoverUserConfig();
            var xsltFile = XsltDirectory / "TransformTrx.xslt";
            var stepConfig = new BuildStepConfig<DotCoverUserConfig>(userConfig, Solution, ArtifactsDirectory, xsltFile);
            new DotCoverStep(stepConfig, InvokedTargets.Contains(Restore), InvokedTargets.Contains(Compile)).Execute();
        });

    Target CompileAndAnalyze => _ => _
        .Produces(ArtifactsDirectory)
        .DependsOn(Compile)
        .DependsOn(GetRoslynAnalyzersResults)
        .DependsOn(CalculateMetrics)
        .DependsOn(RunReSharperInspection)
        .DependsOn(FindDuplications)
        .DependsOn(RunTestsWithCoverage);
}