using System.Text.Json.Serialization;

namespace Components.Analyzer.DupFinder
{
    public class JetBrainsDupFinderUserConfig : IUserConfig
    {
        [JsonIgnore]
        public string StepName => "JetBrainsDupFinder";

        public bool Enabled { get; set; } = true;
    }
}