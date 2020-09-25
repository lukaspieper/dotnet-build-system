using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace BuildSteps
{
    public class BuildStepConfig<T> where T : IBuildStepUserConfig
    {
        public BuildStepConfig(T userConfig, Solution solution, AbsolutePath artifactsDirectory, AbsolutePath xsltFile = null)
        {
            BuildStepUserConfig = userConfig;
            Solution = solution;
            BuildStepArtifactsDirectory = artifactsDirectory / "BuildSteps" / userConfig.StepName;
            XmlReportFile = BuildStepArtifactsDirectory / $"{userConfig.StepName}.xml";
            HtmlReportFile = BuildStepArtifactsDirectory / $"{userConfig.StepName}.html";
            XsltFile = xsltFile;
        }

        public T BuildStepUserConfig;

        public Solution Solution { get; }

        public AbsolutePath BuildStepArtifactsDirectory { get; }

        public AbsolutePath XmlReportFile { get; }

        public AbsolutePath HtmlReportFile { get; }

        public AbsolutePath XsltFile { get; }
    }
}