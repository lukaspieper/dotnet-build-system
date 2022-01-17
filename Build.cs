using System.Collections.Generic;
using System.Diagnostics;
using Components;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using Nuke.Components;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
partial class Build : NukeBuild, IRebuild, IClean, ICopyStaticArtifacts, IHazBuildConfig
{
    public IReadOnlyCollection<Output> MsBuildOutput { get; set; }
    public BuildConfig BuildConfig { get; set; }

    public static int Main()
    {
        return Execute<Build>(build => build.CompileAndAnalyze);
    }

    protected override void OnBuildCreated()
    {
        (this as IHazBuildConfig).OnBuildCreated();
    }

    [UsedImplicitly]
    public Target OpenReport => _ => _
        .Executes(() =>
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = (this as IHazArtifacts).ArtifactsDirectory / "Analysis.html",
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        });
}