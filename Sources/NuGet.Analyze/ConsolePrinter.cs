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

        public void PrintPackageDependenciesForProject(IEnumerable<PackageDependency> projectDependencies, bool verbose)
        {
            foreach (PackageDependency projectDependency in projectDependencies)
            {
                Console.CursorLeft = 10;
                Console.Write(projectDependency.Id);
                Console.CursorLeft = 55;
                Console.WriteLine(projectDependency.VersionSpec);
            }
        }

        public void Log(string message, params object[] args)
        {
            Console.Log(MessageLevel.Info, message, args);
        }
    }
}