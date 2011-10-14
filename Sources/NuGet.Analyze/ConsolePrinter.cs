using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuGet.Common;

namespace NuGet.Analyze
{
    public class ConsolePrinter : IConsolePrinter
    {
        [ImportingConstructor]
        public ConsolePrinter(IConsole console)
        {
            Console = console;
        }

        public IConsole Console { get; private set; }

        public void PrintPackageDependenciesForProject(IEnumerable<PackageDependency> projectDependencies)
        {
            foreach (PackageDependency projectDependency in projectDependencies)
            {
                Console.PrintJustified(10, String.Format("{0} {1}", projectDependency.Id, projectDependency.VersionSpec));
            }
        }

        public void Log(string message, params object[] args)
        {
            Console.Log(MessageLevel.Info, message, args);
        }
    }
}