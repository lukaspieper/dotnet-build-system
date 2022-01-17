using System.Xml.Xsl;
using BuildSteps;
using Nuke.Common.IO;
using Nuke.Components;
using static Nuke.Common.IO.FileSystemTasks;
using static Utilities.XmlTransformation;

namespace Components.Analyzer;

public interface IAnalyzer<T> : IHazReports, IHazCacheDirectory, IHazXsltDirectory where T : IBuildStepUserConfig
{
    protected T UserConfig { get; }

    protected AbsolutePath AnalyzerReportDirectory => ReportDirectory / UserConfig.StepName;
    protected AbsolutePath AnalyzerCacheDirectory => CacheDirectory / UserConfig.StepName;
    protected AbsolutePath XmlReportFile => AnalyzerReportDirectory / $"{UserConfig.StepName}.xml";
    protected AbsolutePath HtmlReportFile => AnalyzerReportDirectory / $"{UserConfig.StepName}.html";
    protected AbsolutePath XsltFile { get; }

    protected void CleanAnalyzerDirectories()
    {
        // Cleaning the analyzer specific directory allows running a step on its own without deleting other results.
        EnsureCleanDirectory(AnalyzerReportDirectory);
        EnsureCleanDirectory(AnalyzerCacheDirectory);
    }

    protected void TransformXmlReportToHtmlReport(XsltArgumentList arguments = null)
    {
        TransformXml(XmlReportFile, XsltFile, HtmlReportFile, arguments);
    }
}