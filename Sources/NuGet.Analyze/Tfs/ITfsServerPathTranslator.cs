using System.ComponentModel.Composition;

namespace NuGet.Analyze.Tfs
{
    [InheritedExport]
    public interface ITfsServerPathTranslator
    {
        string GetServerPath(string baseServerPath, string relativeLocalPath);
    }
}