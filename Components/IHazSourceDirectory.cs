using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

public interface IHazSourceDirectory : INukeBuild
{
    AbsolutePath SourceDirectory => RootDirectory / "src";
}