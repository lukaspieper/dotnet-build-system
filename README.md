# .NET Build System

This repository contains my personal build system for .NET (Core) projects. Currently, the primary goal is to ensure good code quality and consistent formatting. The results of the code analysis are output in a simple HTML report. This project uses [Nuke.build](https://github.com/nuke-build/nuke) as foundation.

## Included build steps for static code analysis

* Roslyn analyzers like [.NET analyzers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.NetAnalyzers/) and [StyleCop analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
* [CodeMetrics](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Metrics/)
* [ReSharper Inspection](https://www.nuget.org/packages/JetBrains.ReSharper.GlobalTools/)
* [ReSharper DupFinder](https://www.nuget.org/packages/JetBrains.ReSharper.GlobalTools/)
* [DotCover](https://www.nuget.org/packages/JetBrains.dotCover.DotNetCliTool/)

Each build step can be disabled or configured through a JSON file.

## Setup

For now the build system is installed as a git submodule.

```
git submodule add -b master https://github.com/lukaspieper/dotnet-build-system.git build
```

## Usage

```
dotnet run --project ./build/_build.csproj
```
