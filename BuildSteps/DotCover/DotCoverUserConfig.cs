using System.Text.Json.Serialization;

namespace BuildSteps.DotCover
{
    public class DotCoverUserConfig : IBuildStepUserConfig
    {
        // Avoids a naming conflict between the test report and the coverage report.
        [JsonIgnore]
        public string StepName => "DotNetTest";

        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     DotCover coverage filter (default: '+:module=*;class=*;function=*;-:module=xunit.assert;')
        /// </summary>
        public string DotCoverCoverageFilter { get; set; } = "+:module=*;class=*;function=*;-:module=xunit.assert;";
    }
}