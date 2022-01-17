using System.IO;
using System.Text.RegularExpressions;
using Nuke.Common.Tools.ReSharper;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.ReSharper.ReSharperTasks;

namespace BuildSteps.ReSharperInspection
{
    public class ReSharperInspectionStep : BuildStep<ReSharperInspectionUserConfig>
    {
        public ReSharperInspectionStep(BuildStepConfig<ReSharperInspectionUserConfig> config) : base(config)
        {
        }

        protected override void ExecuteStep()
        {
            ReSharperInspectCode(_ => _
                .SetTargetPath(Config.Solution)
                .SetOutput(Config.XmlReportFile)
                .SetCachesHome(Config.BuildStepCacheDirectory)
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
                Serilog.Log.Warning($"ReSharper Code Inspection found {numberOfWarnings} warnings.");
            }

            var numberOfSuggestions = Regex.Matches(htmlOutput, "SUGGESTION").Count;
            if (numberOfSuggestions > 0)
            {
                Serilog.Log.Warning($"ReSharper Code Inspection found {numberOfSuggestions} suggestions.");
            }
        }
    }
}