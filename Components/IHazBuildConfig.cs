using System.IO;
using System.Text.Json;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

public interface IHazBuildConfig : INukeBuild
{
    AbsolutePath BuildConfigFile => RootDirectory / ".nuke" / "BuildConfig.json";

    public BuildConfig BuildConfig { get; set; }

    public void OnBuildCreated()
    {
        BuildConfig = GetDeserializedBuildConfigOrDefault();
    }

    [UsedImplicitly]
    Target CreateBuildConfig => _ => _
        .Executes(() =>
        {
            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonText = JsonSerializer.Serialize(new BuildConfig(), serializerOptions);
            File.WriteAllText(BuildConfigFile, jsonText);
        });

    private BuildConfig GetDeserializedBuildConfigOrDefault()
    {
        if (!File.Exists(BuildConfigFile)) return new BuildConfig();

        var jsonText = File.ReadAllText(BuildConfigFile);
        return JsonSerializer.Deserialize<BuildConfig>(jsonText);
    }
}