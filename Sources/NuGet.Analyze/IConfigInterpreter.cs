using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;

namespace NuGet.Analyze
{
    [InheritedExport]
    public interface IConfigInterpreter
    {
        IEnumerable<string> GetRelativePackagesConfigPathsFromRepositoriesConfig(XDocument repositoriesConfigXml);
        IEnumerable<PackageDependency> GetPackageDependenciesFromPackagesConfig(XDocument packagesConfigXml);
    }
}