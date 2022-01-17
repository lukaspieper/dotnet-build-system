using System.Text.Json.Serialization;

namespace Components.Analyzer.DotCover
{
    public class DotCoverUserConfig : IUserConfig
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