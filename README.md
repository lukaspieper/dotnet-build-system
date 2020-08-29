## dotNet Build System

This is my build system for dotNet projects. It is based on [Nuke.Build](https://github.com/nuke-build/nuke).

### Included steps for static code analysis

* Roslyn analyzers like [FxCop](https://www.nuget.org/packages/Microsoft.CodeAnalysis.FxCopAnalyzers/) and [StyleCop](https://www.nuget.org/packages/StyleCop.Analyzers/)
* [CodeMetrics](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Metrics/)
* [ReSharper Inspection](https://www.nuget.org/packages/JetBrains.ReSharper.CommandLineTools/)
* [JetBrains DupFinder](https://www.nuget.org/packages/JetBrains.ReSharper.CommandLineTools/)
* [JetBrains DotCover](https://www.nuget.org/packages/JetBrains.dotCover.CommandLineTools/)
