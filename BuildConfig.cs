using BuildSteps.CodeMetrics;
using BuildSteps.DotCover;
using BuildSteps.JetBrainsDupFinder;
using BuildSteps.ReSharperInspection;
using BuildSteps.RoslynAnalyzers;

public class BuildConfig
{
    public CodeMetricsUserConfig CodeMetricsUserConfig { get; set; } = new CodeMetricsUserConfig();

    public DotCoverUserConfig DotCoverUserConfig { get; set; } = new DotCoverUserConfig();

    public JetBrainsDupFinderUserConfig JetBrainsDupFinderUserConfig { get; set; } = new JetBrainsDupFinderUserConfig();

    public ReSharperInspectionUserConfig ReSharperInspectionUserConfig { get; set; } = new ReSharperInspectionUserConfig();

    public RoslynAnalyzersUserConfig RoslynAnalyzersUserConfig { get; set; } = new RoslynAnalyzersUserConfig();
}