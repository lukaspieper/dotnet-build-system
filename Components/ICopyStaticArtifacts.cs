using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Components;
using static Nuke.Common.IO.FileSystemTasks;

namespace Components;

public interface ICopyStaticArtifacts : IHazBuildDirectory, IHazArtifacts
{
    [UsedImplicitly]
    Target CopyStaticArtifacts => _ => _
        .After<IClean>()
        .Executes(() =>
        {
            EnsureExistingDirectory(ArtifactsDirectory);

            CopyDirectoryRecursively(BuildDirectory / "StaticArtifacts",
                ArtifactsDirectory,
                DirectoryExistsPolicy.Merge,
                FileExistsPolicy.OverwriteIfNewer);
        });
}