using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

public interface IHazBuildDirectory : INukeBuild
{
    AbsolutePath BuildDirectory => RootDirectory / "build";
}