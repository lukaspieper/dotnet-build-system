using System.IO;
using System.Text.RegularExpressions;
using Nuke.Common.Tools.InspectCode;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.InspectCode.InspectCodeTasks;

namespace BuildSteps.ReSharperInspection
{
    public class ReSharperInspectionStep : BuildStep<ReSharperInspectionUserConfig>
    {
        public ReSharperInspectionStep(BuildStepConfig<ReSharperInspectionUserConfig> config) : base(config)
        {
        }

        protected override void ExecuteStep()
        {
            InspectCode(_ => _
                .SetTargetPath(Config.Solution)
                .SetOutput(Config.XmlReportFile)
            );

            TransformXmlReportToHtmlReport();
            LogCodeInspectionResults();
        }

        private void LogCodeInspectionResults()
        {
            var htmlOutput = File.ReadAllText(Config.HtmlReportFile);

            var numberOfWarnings = Regex.Matches(htmlOutput, "WARNING").Count;
            if (numberOfWarnings > 0)
            {
                Warn($"ReSharper Code Inspection found {numberOfWarnings} warnings.");
            }

            var numberOfSuggestions = Regex.Matches(htmlOutput, "SUGGESTION").Count;
            if (numberOfSuggestions > 0)
            {
                Warn($"ReSharper Code Inspection found {numberOfSuggestions} suggestions.");
            }
        }
    }
}