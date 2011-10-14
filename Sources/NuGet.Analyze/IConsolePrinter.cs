using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuGet.Common;

namespace NuGet.Analyze
{
    [InheritedExport]
    public interface IConsolePrinter
    {
        IConsole Console { get; }

        void PrintPackageDependenciesForProject(IEnumerable<PackageDependency> projectDependencies, bool verbose);
        void Log(string message, params object[] args);
    }
}