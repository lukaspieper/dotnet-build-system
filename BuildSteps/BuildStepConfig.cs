using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace BuildSteps
{
    public class BuildStepConfig<T> where T : IBuildStepUserConfig
    {
        public T BuildStepUserConfig;

        public BuildStepConfig(T userConfig, Solution solution, AbsolutePath artifactsDirectory, AbsolutePath cacheDirectory, AbsolutePath xsltFile = null)
        {
            BuildStepUserConfig = userConfig;
            Solution = solution;

            BuildStepArtifactsDirectory = artifactsDirectory / "BuildSteps" / userConfig.StepName;
            BuildStepCacheDirectory = cacheDirectory / userConfig.StepName;

            XmlReportFile = BuildStepArtifactsDirectory / $"{userConfig.StepName}.xml";
            HtmlReportFile = BuildStepArtifactsDirectory / $"{userConfig.StepName}.html";
            XsltFile = xsltFile;
        }

        public Solution Solution { get; }

        public AbsolutePath BuildStepArtifactsDirectory { get; }

        public AbsolutePath BuildStepCacheDirectory { get; }

        public AbsolutePath XmlReportFile { get; }

        public AbsolutePath HtmlReportFile { get; }

        public AbsolutePath XsltFile { get; }
    }
}