using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
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
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] Solution Solution { get; }

    BuildConfig BuildConfig { get; set; }

    AbsolutePath BuildDirectory => RootDirectory / "build";
    AbsolutePath XsltDirectory => BuildDirectory / "Xslt";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    public static int Main() => Execute<Build>(build => build.CompileAndAnalyze);

    protected override void OnBuildCreated() => BuildConfig = GetDeserializedBuildConfigOrDefault();

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
}