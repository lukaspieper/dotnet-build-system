using System.Collections.Generic;
using System.Diagnostics;
using Components;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Components;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
partial class Build : NukeBuild, IRebuild, IClean, ICopyStaticArtifacts, IHazBuildConfig
{
    public IReadOnlyCollection<Output> MsBuildOutput { get; set; }
    public BuildConfig BuildConfig { get; set; }
    public AbsolutePath XsltDirectory => (this as IHazBuildDirectory).BuildDirectory / "Xslt";

    public static int Main()
    {
        return Execute<Build>(build => build.CompileAndAnalyze);
    }

    protected override void OnBuildCreated()
    {
        BuildConfig = (this as IHazBuildConfig).GetDeserializedBuildConfigOrDefault();
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