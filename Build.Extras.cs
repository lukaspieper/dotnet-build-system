using System.Diagnostics;
using System.IO;
using System.Text.Json;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;

public partial class Build
{
    AbsolutePath BuildConfigFile => RootDirectory / "BuildConfig.json";

    [UsedImplicitly]
    Target OpenReport => _ => _
        .Executes(() =>
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = ArtifactsDirectory / "Analysis.html",
                UseShellExecute = true,
            };

            Process.Start(processStartInfo);
        });

    [UsedImplicitly]
    Target CreateBuildConfig => _ => _
        .Executes(() =>
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };

            var jsonText = JsonSerializer.Serialize(new BuildConfig(), serializerOptions);
            File.WriteAllText(BuildConfigFile, jsonText);
        });

    private BuildConfig GetDeserializedBuildConfigOrDefault()
    {
        if (File.Exists(BuildConfigFile))
        {
            var jsonText = File.ReadAllText(BuildConfigFile);
            return JsonSerializer.Deserialize<BuildConfig>(jsonText);
        }
        
        return new BuildConfig();
    }
}