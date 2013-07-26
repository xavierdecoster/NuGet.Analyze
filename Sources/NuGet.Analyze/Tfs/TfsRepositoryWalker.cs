using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace NuGet.Analyze.Tfs
{
    public class TfsRepositoryWalker : IRepositoryWalker
    {
        private const string repositoriesConfigFileName = "repositories.config";
        private readonly IConfigInterpreter configInterpreter;
        private readonly ITfsServerPathTranslator tfsServerPathTranslator;

        [ImportingConstructor]
        public TfsRepositoryWalker(IConsolePrinter consolePrinter, IConfigInterpreter configInterpreter, ITfsServerPathTranslator tfsServerPathTranslator)
        {
            ConsolePrinter = consolePrinter;
            this.configInterpreter = configInterpreter;
            this.tfsServerPathTranslator = tfsServerPathTranslator;
        }

        public IConsolePrinter ConsolePrinter { get; private set; }

        public RepositoryType CommandAction { get { return RepositoryType.Tfs; } }

        public void AnalyzeRepository(string repository, bool verbose)
        {
            ConsolePrinter.Log("Analyzing TFS Source Control on server '{0}'", repository);
            if (String.IsNullOrWhiteSpace(repository))
            {
                throw new CommandLineException(AnalyzeCommandResources.AnalyzeCommandRepositoryRequired);
            }

            Uri tfsUri = new Uri(repository);
            TfsConfigurationServer tfs = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri, new UICredentialsProvider());
            tfs.EnsureAuthenticated();

            IDictionary<TfsTeamProjectCollection, ReadOnlyCollection<CatalogNode>> teamProjectCollectionInfo
                = GetTeamProjectCollectionsWithTeamProjectCatalogNodes(tfs);

            foreach (KeyValuePair<TfsTeamProjectCollection, ReadOnlyCollection<CatalogNode>> teamProjectCollectionKvp in teamProjectCollectionInfo)
            {
                AnalyzeTfsTeamProjectCollection(teamProjectCollectionKvp, verbose);
            }
        }

        private void AnalyzeTfsTeamProjectCollection(KeyValuePair<TfsTeamProjectCollection, ReadOnlyCollection<CatalogNode>> teamProjectCollectionKvp, bool verbose)
        {
            TfsTeamProjectCollection tpc = teamProjectCollectionKvp.Key;
            ConsolePrinter.Log("Team Project Collection '{0}'", tpc.Name);

            // Note: this requires a dependency to Microsoft.TeamFoundation.VersionControl.Client. Really needed?
            VersionControlServer vcs = tpc.GetService<VersionControlServer>();
            TeamProject[] teamProjects = vcs.GetAllTeamProjects(true);

            foreach (TeamProject teamProject in teamProjects)
            {
                ConsolePrinter.Log(" -- Team Project '{0}'", teamProject.Name);

                AnalyzeTfsTeamProject(teamProject, verbose);
            }
        }

        private void AnalyzeTfsTeamProject(TeamProject teamProject, bool verbose)
        {
            List<PackageDependency> teamProjectDependencies = new List<PackageDependency>();
            string repositorySearchQuery = string.Format("$/{0}/*{1}", teamProject.Name, repositoriesConfigFileName);
            ItemSet repositories = teamProject.VersionControlServer.GetItems(repositorySearchQuery, RecursionType.Full);
            foreach (Item repository in repositories.Items)
            {
                IEnumerable<PackageDependency> repositoryDependencies = GetPackageDependenciesFromRepositoriesConfig(teamProject, repository);
                foreach (PackageDependency packageDependency in repositoryDependencies)
                {
                    if (teamProjectDependencies.FirstOrDefault(dp => dp.Id == packageDependency.Id && dp.VersionSpec == packageDependency.VersionSpec) == null)
                        teamProjectDependencies.Add(packageDependency);
                }
            }

            ConsolePrinter.PrintPackageDependenciesForProject(teamProjectDependencies, verbose);
        }

        private IEnumerable<PackageDependency> GetPackageDependenciesFromRepositoriesConfig(TeamProject teamProject, Item repository)
        {
            List<PackageDependency> repositoryDependencies = new List<PackageDependency>();

            Stream repoFileStream = repository.DownloadFile();
            XDocument repoXml = XDocument.Load(repoFileStream);
            IEnumerable<string> relativePackageConfigPaths = configInterpreter.GetRelativePackagesConfigPathsFromRepositoriesConfig(repoXml);
            string repositoriesConfigServerPath = repository.ServerItem;
            int repositoriesConfigLength = repositoriesConfigFileName.Length;
            string packageConfigFolderServerPath = repositoriesConfigServerPath.Remove(repositoriesConfigServerPath.Length - repositoriesConfigLength, repositoriesConfigLength);

            foreach (string packageConfigPathRelativeToRepositoriesConfig in relativePackageConfigPaths)
            {
                // navigate to the folder server path for the repositories.config file
                string repositoryNodeServerItem = tfsServerPathTranslator.GetServerPath(packageConfigFolderServerPath, packageConfigPathRelativeToRepositoriesConfig);
                Item packageConfig = teamProject.VersionControlServer.GetItem(repositoryNodeServerItem);
                if (packageConfig != null)
                {
                    Stream packagesConfigStream = packageConfig.DownloadFile();
                    XDocument packagesConfigXml = XDocument.Load(packagesConfigStream);
                    IEnumerable<PackageDependency> packageDependencies = configInterpreter.GetPackageDependenciesFromPackagesConfig(packagesConfigXml);
                    foreach (PackageDependency packageDependency in packageDependencies)
                    {
                        if (repositoryDependencies.Where(dp => dp.Id == packageDependency.Id && dp.VersionSpec == packageDependency.VersionSpec).FirstOrDefault() == null)
                            repositoryDependencies.Add(packageDependency);
                    }
                }
            }
            
            return repositoryDependencies;
        }

        private IDictionary<TfsTeamProjectCollection, ReadOnlyCollection<CatalogNode>> GetTeamProjectCollectionsWithTeamProjectCatalogNodes(TfsConfigurationServer tfs)
        {
            CatalogNode catalogNode = tfs.CatalogNode;

            // Note: this requires a dependency to Microsoft.TeamFoundation.Common. Really needed?
            ReadOnlyCollection<CatalogNode> tpcNodes =
                catalogNode.QueryChildren(new[] { CatalogResourceTypes.ProjectCollection }, false, CatalogQueryOptions.None);

            var tpcDictionary = new Dictionary<TfsTeamProjectCollection, ReadOnlyCollection<CatalogNode>>();
            foreach (CatalogNode tpcNode in tpcNodes)
            {
                Guid instanceId = new Guid(tpcNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection tpc = tfs.GetTeamProjectCollection(instanceId);

                // Get catalog of tp = 'Team Projects' for the tpc = 'Team Project Collection'
                ReadOnlyCollection<CatalogNode> tpNodes =
                    tpcNode.QueryChildren(new[] { CatalogResourceTypes.TeamProject }, false, CatalogQueryOptions.None);

                tpcDictionary.Add(tpc, new ReadOnlyCollection<CatalogNode>(tpNodes));
            }

            return tpcDictionary;
        }

    }
}