using System.Collections.Generic;
using Components;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
partial class Build : NukeBuild, IRebuild, IClean, ICopyStaticArtifacts
{
    public IReadOnlyCollection<Output> MsBuildOutput { get; set; }
    
    BuildConfig BuildConfig { get; set; }
    
    AbsolutePath XsltDirectory => (this as IHazBuildDirectory).BuildDirectory / "Xslt";

    public static int Main() => Execute<Build>(build => build.CompileAndAnalyze);

    protected override void OnBuildCreated() => BuildConfig = GetDeserializedBuildConfigOrDefault();
}