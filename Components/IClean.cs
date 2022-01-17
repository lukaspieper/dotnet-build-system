using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using Nuke.Components;
using static Nuke.Common.IO.FileSystemTasks;

namespace Components;

public interface IClean : IHazSourceDirectory, IHazCacheDirectory, IHazArtifacts
{
    [UsedImplicitly]
    Target Clean => _ => _
        .Before<IRestore>()
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj", "**/AppPackages").ForEach(DeleteDirectory);
            
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(CacheDirectory);
        });
}