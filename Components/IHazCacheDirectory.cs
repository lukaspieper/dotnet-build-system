using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

public interface IHazCacheDirectory : INukeBuild
{
    AbsolutePath CacheDirectory => RootDirectory / ".cache";
}