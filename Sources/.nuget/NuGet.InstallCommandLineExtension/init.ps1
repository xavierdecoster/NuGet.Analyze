param($installPath, $toolsPath, $package, $project)
Import-Module (Join-Path $toolsPath NuGet.InstallCommandLineExtension.psd1)

# Install the command line extension to be able to add a console extension using the console :-)
Install-CommandLineExtension AddConsoleExtension

Write-Host ""
Write-Host "*************************************************************************************"
Write-Host " INSTRUCTIONS"
Write-Host "*************************************************************************************"
Write-Host " - To install a NuGet command line extension, use the Install-CommandLineExtension command."
Write-Host " - E.g.: Install-CommandLineExtension NuGet.Analyze"
Write-Host "*************************************************************************************"
Write-Host ""