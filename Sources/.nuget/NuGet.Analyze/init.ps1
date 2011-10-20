param($installPath, $toolsPath, $package, $project)
Import-Module (Join-Path $toolsPath NuGet.Analyze.psd1)

Write-Host ""
Write-Host "*************************************************************************************"
Write-Host " INSTRUCTIONS"
Write-Host "*************************************************************************************"
Write-Host " - To install the NuGet command line extension, use the 'Install-CommandLineExtension NuGet.Analyze' command.
# Write-Host " - To analyze a TFS source control repository, use the 'Analyze-TFS' command."
# Write-Host " - To analyze a file system repository, use the 'Analyze-FileSystem' command."
# Write-Host " - For for information, see https://github.com/xavierdecoster/NuGet.Analyze"
Write-Host "*************************************************************************************"
Write-Host ""