using System.Text.Json.Serialization;

namespace Components.Analyzer.ReSharper
{
    public class ReSharperInspectionUserConfig : IUserConfig
    {
        [JsonIgnore]
        public string StepName => "ReSharperInspection";

        public bool Enabled { get; set; } = true;
    }
}