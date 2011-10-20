using System;
using System.Management.Automation;
using NuGet.PowerShell.Commands;
using NuGet.VisualStudio;

namespace NuGet.Analyze.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "DependencyMatrix")]
    public class AnalyzeCmdlet : NuGetBaseCommand 
    {
        public AnalyzeCmdlet(ISolutionManager solutionManager, IVsPackageManagerFactory vsPackageManagerFactory, IHttpClientEvents httpClientEvents) 
            : base(solutionManager, vsPackageManagerFactory, httpClientEvents)
        {
        }

        protected override void ProcessRecordCore()
        {
            throw new NotImplementedException();
        }
    }
}