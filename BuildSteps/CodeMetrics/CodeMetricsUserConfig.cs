using System.Text.Json.Serialization;

namespace BuildSteps.CodeMetrics
{
    public class CodeMetricsUserConfig : IBuildStepUserConfig
    {
        [JsonIgnore]
        public string StepName => "CodeMetrics";

        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     Minimal value for CodeMetrics' MaintainabilityIndex. If a member is rated lower, the process fails.
        /// </summary>
        public int MaintainabilityIndexMinimum { get; set; } = 60;
    }
}