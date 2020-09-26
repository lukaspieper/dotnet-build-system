using System.Text.Json.Serialization;

namespace BuildSteps.JetBrainsDupFinder
{
    public class JetBrainsDupFinderUserConfig : IBuildStepUserConfig
    {
        [JsonIgnore]
        public string StepName => "JetBrainsDupFinder";

        public bool Enabled { get; set; } = true;
    }
}