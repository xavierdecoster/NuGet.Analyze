using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using NuGet.Commands;

namespace NuGet.Analyze
{
    [Command(typeof(AnalyzeCommandResources), "analyze", "AnalyzeCommandDescription",
        UsageSummaryResourceName = "AnalyzeCommandUsageSummary", MinArgs = 0, MaxArgs = 1, AltName = "analyse")]
    public class AnalyzeCommand : Command
    {
        private readonly IEnumerable<IRepositoryWalker> sourceControlWalkers;

        [ImportingConstructor]
        public AnalyzeCommand([ImportMany]IEnumerable<IRepositoryWalker> sourceControlWalkers)
        {
            this.sourceControlWalkers = sourceControlWalkers;
        }

        [Option(typeof(AnalyzeCommandResources), "AnalyzeCommandRepositoryDescription")]
        public string Repository { get; set; }

        [Option(typeof(AnalyzeCommandResources), "AnalyzeCommandVerboseDescription")]
        public bool Verbose { get; set; }

        public override void ExecuteCommand()
        {
            RepositoryType action = Arguments.Any() 
                ? (RepositoryType)Enum.Parse(typeof (RepositoryType), Arguments.First(), true) 
                : RepositoryType.Tfs;

            IRepositoryWalker selectedRepositoryWalker = sourceControlWalkers
                .Where(scw => scw.CommandAction == action)
                .FirstOrDefault();

            if (selectedRepositoryWalker != null)
                selectedRepositoryWalker.AnalyzeRepository(Repository, Verbose);
            else throw new CommandLineException(AnalyzeCommandResources.AnalyzeCommandUnknownCommand);
        }
    }
}
