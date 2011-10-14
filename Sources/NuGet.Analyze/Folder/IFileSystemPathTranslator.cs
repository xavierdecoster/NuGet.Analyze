using System.ComponentModel.Composition;
using System.IO;

namespace NuGet.Analyze.Folder
{
    [InheritedExport]
    public interface IFileSystemPathTranslator
    {
        FileInfo GetAbsolutePath(FileInfo sourcePath, string relativeTargetPath);
    }
}