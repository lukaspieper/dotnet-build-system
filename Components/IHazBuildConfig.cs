using System.IO;
using System.Text.Json;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

public interface IHazBuildConfig : INukeBuild
{
    AbsolutePath BuildConfigFile => RootDirectory / "BuildConfig.json";
    
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

    public BuildConfig GetDeserializedBuildConfigOrDefault()
    {
        if (!File.Exists(BuildConfigFile)) return new BuildConfig();
        
        var jsonText = File.ReadAllText(BuildConfigFile);
        return JsonSerializer.Deserialize<BuildConfig>(jsonText);
    }
}