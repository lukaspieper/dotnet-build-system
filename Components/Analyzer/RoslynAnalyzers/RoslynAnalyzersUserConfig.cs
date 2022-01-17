﻿using System.Text.Json.Serialization;

namespace Components.Analyzer.RoslynAnalyzers
{
    public class RoslynAnalyzersUserConfig : IUserConfig
    {
        [JsonIgnore]
        public string StepName => "RoslynAnalyzers";

        public bool Enabled { get; set; } = true;

        /// <summary>
        ///     Maximal number of Roslyn analyzers warnings. If the number is higher, the process fails.
        /// </summary>
        public int RoslynAnalyzersWarningThreshold { get; set; }
    }
}