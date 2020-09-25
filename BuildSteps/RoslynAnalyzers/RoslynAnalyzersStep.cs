using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Nuke.Common.Tooling;
using static Nuke.Common.ControlFlow;
using static Nuke.Common.Logger;

namespace BuildSteps.RoslynAnalyzers
{
    public class RoslynAnalyzersStep : BuildStep<RoslynAnalyzersUserConfig>
    {
        private readonly Regex AnalyzerResultRegex = new Regex(@"^\s*(?'Location'\S*[\(\d*,\d*\)]?)\s*:\s*(?'Severity'[a-z]*)\s*(?'Code'[A-Z]{2}\d{4}):\s*(?'Description'.*)\s\[(?'Project'.*)\]");
        private readonly IReadOnlyCollection<Output> MsBuildOutput;

        public RoslynAnalyzersStep(BuildStepConfig<RoslynAnalyzersUserConfig> config, IReadOnlyCollection<Output> msBuildOutput) : base(config)
        {
            MsBuildOutput = msBuildOutput;
        }

        protected override void ExecuteStep()
        {
            var analyzerResults = GetRoslynAnalyzersResultsFromBuildOutput();

            WriteResultsToXmlFile(analyzerResults);
            TransformXmlReportToHtmlReport();

            if (analyzerResults.Count > UserConfig.RoslynAnalyzersWarningThreshold)
            {
                Fail($"RoslynAnalyzers found too many warnings ({analyzerResults.Count}, max. {UserConfig.RoslynAnalyzersWarningThreshold}).");
            }
            else if (analyzerResults.Count > 0)
            {
                Warn($"RoslynAnalyzers found {analyzerResults.Count} warnings.");
            }
        }

        private List<AnalyzerResult> GetRoslynAnalyzersResultsFromBuildOutput()
        {
            return MsBuildOutput.Select(o => o.Text.Trim())
                .Distinct()
                .Select(line => AnalyzerResultRegex.Match(line))
                .Where(match => match.Success)
                .Select(match => new AnalyzerResult(match))
                .OrderBy(result => result.Location)
                .ToList();
        }

        private void WriteResultsToXmlFile(List<AnalyzerResult> results)
        {
            var serializer = new XmlSerializer(typeof(List<AnalyzerResult>), new XmlRootAttribute("AnalyzerResults"));

            using var writer = new StreamWriter(Config.XmlReportFile);
            serializer.Serialize(writer, results);
        }
    }
}