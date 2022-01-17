using Components.Analyzer.CodeMetrics;
using Components.Analyzer.DotCover;
using Components.Analyzer.DupFinder;
using Components.Analyzer.ReSharper;
using Components.Analyzer.RoslynAnalyzers;
using Nuke.Common;

namespace Components.Analyzer;

public interface IAllAnalyzer : ICodeMetrics, IRoslynAnalyzers, IReSharperInspection, IDotCover, IDupFinder
{
    Target CompileAndAnalyze => _ => _
        .Produces(ArtifactsDirectory)
        .DependsOn<IRebuild>()
        .DependsOn<IRoslynAnalyzers>()
        .DependsOn<ICodeMetrics>()
        .DependsOn<IReSharperInspection>()
        .DependsOn<IDupFinder>()
        .DependsOn<IDotCover>();
}