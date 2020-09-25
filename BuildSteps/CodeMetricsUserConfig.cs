using System.Text.Json.Serialization;

namespace BuildSteps
{
    public class CodeMetricsUserConfig : IBuildStepUserConfig
    {
        [JsonIgnore]
        public string StepName => "CodeMetrics";

        /// <summary>
        ///     Minimal value for CodeMetrics' MaintainabilityIndex. If a member is rated lower, the process fails.
        /// </summary>
        public int MaintainabilityIndexMinimum { get; set; } = 60;
    }
}