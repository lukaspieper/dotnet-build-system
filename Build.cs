using System;
using System.Collections.Generic;
using System.Diagnostics;
using Components;
using Components.Analyzer;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Nuke.Components;

[UnsetVisualStudioEnvironmentVariables]
public class Build : NukeBuild, IClean, ICopyStaticArtifacts, IAllAnalyzer
{
    public IReadOnlyCollection<Output> MsBuildOutput { get; set; }
    public BuildConfig BuildConfig { get; set; }

    public static int Main()
    {
        return Execute<Build>(build => (build as IAllAnalyzer).CompileAndAnalyze);
    }

    protected override void OnBuildCreated()
    {
        (this as IHazBuildConfig).OnBuildCreated();
    }

    protected override void WriteLogo()
    {
        new[]
        {
            "",
            "██████╗ ██╗  ██████╗ ██╗  ███████╗",
            "██╔══██╗╚██╗ ██╔══██╗╚██╗ ██╔════╝",
            "██║  ██║ ╚██╗██████╔╝ ╚██╗███████╗",
            "██║  ██║ ██╔╝██╔══██╗ ██╔╝╚════██║",
            "██████╔╝██╔╝ ██████╔╝██╔╝ ███████║",
            "╚═════╝ ╚═╝  ╚═════╝ ╚═╝  ╚══════╝",
            ""
        }.ForEach(x => Console.WriteLine(x.Replace(" ", " ")));

        Console.WriteLine($"NUKE Execution Engine {typeof(NukeBuild).Assembly.GetInformationalText()}");
        Console.WriteLine($"dotnet-build-system   {typeof(Build).Assembly.GetInformationalText()}");
        Console.WriteLine();
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