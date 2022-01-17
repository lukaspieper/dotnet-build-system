using Nuke.Common.IO;

namespace Components;

public interface IHazXsltDirectory : IHazBuildDirectory
{
    public AbsolutePath XsltDirectory => BuildDirectory / "Xslt";
}