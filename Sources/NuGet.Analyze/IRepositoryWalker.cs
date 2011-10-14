using System.ComponentModel.Composition;

namespace NuGet.Analyze
{
    [InheritedExport]
    public interface IRepositoryWalker
    {
        IConsolePrinter ConsolePrinter { get; }
        RepositoryType CommandAction { get; }
        void AnalyzeRepository(string repository, bool verbose);
    }
}