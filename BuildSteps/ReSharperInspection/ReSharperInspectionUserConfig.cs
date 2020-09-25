using System.Text.Json.Serialization;

namespace BuildSteps.ReSharperInspection
{
    public class ReSharperInspectionUserConfig : IBuildStepUserConfig
    {
        [JsonIgnore]
        public string StepName => "ReSharperInspection";
    }
}