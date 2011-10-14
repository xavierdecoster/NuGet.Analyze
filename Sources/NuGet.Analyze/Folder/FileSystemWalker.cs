using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NuGet.Analyze.Folder
{
    public class FileSystemWalker : IRepositoryWalker
    {
        private readonly IConfigInterpreter configInterpreter;
        private readonly IFileSystemPathTranslator fileSystemPathTranslator;

        [ImportingConstructor]
        public FileSystemWalker(IConsolePrinter consolePrinter, IConfigInterpreter configInterpreter, IFileSystemPathTranslator fileSystemPathTranslator)
        {
            this.configInterpreter = configInterpreter;
            this.fileSystemPathTranslator = fileSystemPathTranslator;
            ConsolePrinter = consolePrinter;
        }

        public IConsolePrinter ConsolePrinter { get; private set; }

        public RepositoryType CommandAction { get { return RepositoryType.FileSystem; } }

        public void AnalyzeRepository(string repository, bool verbose)
        {
            // validate arguments
            ConsolePrinter.Log("Analyzing file system path '{0}'", repository);
            if (String.IsNullOrWhiteSpace(repository))
            {
                throw new CommandLineException(AnalyzeCommandResources.AnalyzeCommandRepositoryRequired);
            }

            // get all the solutions in this directory
            IEnumerable<FileInfo> solutions = GetSolutionsInDirectory(repository);

            // analyze each solution
            foreach (FileInfo solution in solutions)
            {
                if (verbose)
                    ConsolePrinter.Log("Solution '{0}' in folder '{1}'", solution.Name, solution.DirectoryName);
                else ConsolePrinter.Log("Solution '{0}'", solution.Name);
                AnalyzePackagesRepository(solution, verbose);
            }
        }

        internal void AnalyzePackagesRepository(FileInfo solution, bool verbose)
        {
            if (solution.Directory == null)
                return;

            //TODO: packages path might be configurable
            string repositoriesConfigPath = Path.Combine(solution.Directory.FullName, @"packages\repositories.config");
            if (File.Exists(repositoriesConfigPath))
            {
                FileInfo repositoriesConfig = new FileInfo(repositoriesConfigPath);
                IEnumerable<string> relativePackagesConfigPaths;
                using (var repositoriesConfigStream = File.OpenRead(repositoriesConfig.FullName))
                {
                    relativePackagesConfigPaths =
                        configInterpreter.GetRelativePackagesConfigPathsFromRepositoriesConfig(
                            XDocument.Load(repositoriesConfigStream));
                }

                if (relativePackagesConfigPaths != null)
                {
                    foreach (string relativePackagesConfigPath in relativePackagesConfigPaths)
                    {
                        FileInfo packagesConfig = fileSystemPathTranslator.GetAbsolutePath(repositoriesConfig, relativePackagesConfigPath);
                        string projectName = GetProjectNameForPackagesConfig(solution, repositoriesConfig, packagesConfig);
                        if (!string.IsNullOrWhiteSpace(projectName))
                        {
                            if (verbose)
                                ConsolePrinter.Log(" -- Project '{0}' in folder '{1}'", projectName, packagesConfig.DirectoryName);
                            else ConsolePrinter.Log(" -- Project '{0}'", projectName);
                        }

                        using (var packagesConfigStream = File.OpenRead(packagesConfig.FullName))
                        {
                            IEnumerable<PackageDependency> packageDependencies =
                                configInterpreter.GetPackageDependenciesFromPackagesConfig(XDocument.Load(packagesConfigStream));
                            ConsolePrinter.PrintPackageDependenciesForProject(packageDependencies, verbose);
                        }
                    }
                }
            }
        }

        internal string GetProjectNameForPackagesConfig(FileInfo solution, FileInfo repositoriesConfig, FileInfo packagesConfig)
        {
            if (packagesConfig.DirectoryName == null)
                return null;

            string projectName = packagesConfig.DirectoryName.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar).LastOrDefault();
            if (projectName != null)
            {
                return projectName;
            }
            return null;
        }

        internal DirectoryInfo GetRepositoryWorkingDirectory(string repository)
        {
            string directory = Path.GetDirectoryName(repository);
            if (directory != null && Directory.Exists(directory))
            {
                return new DirectoryInfo(directory);
            }
            return null;
        }

        internal IEnumerable<FileInfo> GetSolutionsInDirectory(string repository)
        {
            DirectoryInfo workingDirectory = GetRepositoryWorkingDirectory(repository);
            if (workingDirectory == null)
            {
                throw new CommandLineException(AnalyzeCommandResources.AnalyzeCommandRepositoryNotFound);
            }

            return Directory.GetFiles(workingDirectory.FullName, "*.sln", SearchOption.AllDirectories)
                .Select(sln => new FileInfo(sln)).ToList();
        }
    }
}