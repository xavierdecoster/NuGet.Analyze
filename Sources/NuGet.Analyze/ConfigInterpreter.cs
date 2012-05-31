using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NuGet.Analyze
{
    public class ConfigInterpreter : IConfigInterpreter
    {
        public IEnumerable<string> GetRelativePackagesConfigPathsFromRepositoriesConfig(XDocument repositoriesConfigXml)
        {
            List<string> relativePackageConfigPaths = new List<string>();
            if (repositoriesConfigXml.Root != null)
            {
                IEnumerable<XElement> repositoryNodes = repositoriesConfigXml.Root.Elements();
                foreach (XElement repositoryNode in repositoryNodes)
                {
                    XAttribute pathAttribute = repositoryNode.Attribute("path");
                    if (pathAttribute != null)
                    {
                        string packageConfigPathRelativeToRepositoriesConfig = pathAttribute.Value;
                        relativePackageConfigPaths.Add(packageConfigPathRelativeToRepositoriesConfig);
                    }
                }
            }

            return relativePackageConfigPaths;
        }

        public IEnumerable<PackageDependency> GetPackageDependenciesFromPackagesConfig(XDocument packagesConfigXml)
        {
            List<PackageDependency> packageDependencies = new List<PackageDependency>();
            if (packagesConfigXml.Root != null)
            {
                foreach (XElement packageElement in packagesConfigXml.Root.Elements("package"))
                {
                    XAttribute idAttribute = packageElement.Attribute("id");
                    XAttribute versionAttribute = packageElement.Attribute("version");
                    if (idAttribute != null && versionAttribute != null)
                    {
                        string id = idAttribute.Value;
                        IVersionSpec versionSpec = new VersionSpec(new SemanticVersion(versionAttribute.Value));
                        PackageDependency packageInfo = new PackageDependency(id, versionSpec);
                        packageDependencies.Add(packageInfo);
                    }
                }
            }
            return packageDependencies;
        }
    }
}