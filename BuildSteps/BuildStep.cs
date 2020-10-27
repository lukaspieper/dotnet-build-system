using System.Xml.Xsl;
using static Nuke.Common.IO.FileSystemTasks;
using static Utilities.XmlTransformation;

namespace BuildSteps
{
    public abstract class BuildStep<T> where T : IBuildStepUserConfig
    {
        protected BuildStep(BuildStepConfig<T> config)
        {
            Config = config;
        }

        protected BuildStepConfig<T> Config { get; }

        protected T UserConfig => Config.BuildStepUserConfig;

        public void Execute()
        {
            // Cleaning the build step specific directory allows running a step on its own without deleting other results.
            EnsureCleanDirectory(Config.BuildStepArtifactsDirectory);
            EnsureCleanDirectory(Config.BuildStepCacheDirectory);

            ExecuteStep();
        }

        protected abstract void ExecuteStep();

        protected void TransformXmlReportToHtmlReport(XsltArgumentList arguments = null)
        {
            TransformXml(Config.XmlReportFile, Config.XsltFile, Config.HtmlReportFile, arguments);
        }
    }
}