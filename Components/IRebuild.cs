using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Components;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

namespace Components;

public interface IRebuild : ICompile
{
    IReadOnlyCollection<Output> MsBuildOutput { set; }

    new Target Compile => _ => _
        .DependsOn<IClean>()
        .DependsOn<IRestore>()
        .Executes(() =>
        {
            MsBuildOutput = MSBuild(s => s
                .SetTargetPath(Solution)
                .SetConfiguration(Configuration));
        });
}