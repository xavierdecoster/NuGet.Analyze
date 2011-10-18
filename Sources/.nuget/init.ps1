function Get-SolutionDir {
    if($dte.Solution -and $dte.Solution.IsOpen) {
        return Split-Path $dte.Solution.Properties.Item("Path").Value
    }
    else {
        throw "Solution not avaliable"
    }
}
function Get-InstallPath {
    param(
        $package
    )
    # Get the repository path
    $componentModel = Get-VSComponentModel
    $repositorySettings = $componentModel.GetService([NuGet.VisualStudio.IRepositorySettings])
    $pathResolver = New-Object NuGet.DefaultPackagePathResolver($repositorySettings.RepositoryPath)
    $pathResolver.GetInstallPath($package)
}

function ConfigureNuGetCommandLine {
    # Install the nuget command line if it doesn't exist
    $solutionDir = Get-SolutionDir
    $nugetToolsPath = (Join-Path $solutionDir .tmp)
    
    #if(!(Test-Path $nugetToolsPath) -or !(Get-ChildItem $nugetToolsPath)) {

        Install-Package NuGet.CommandLine -Source 'https://go.microsoft.com/fwlink/?LinkID=206669'
        
        $nugetExePackage = @(Get-Package NuGet.CommandLine)[0]
        
        if(!$nugetExePackage) {
            return $false
        }
        
        # Get the package path
        $nugetExePath = Get-InstallPath $nugetExePackage
        if(!(Test-Path $nugetToolsPath)) {
            mkdir $nugetToolsPath | Out-Null
        }

        Copy-Item "$nugetExePath\tools\*.*" $nugetToolsPath -Force | Out-Null

		Uninstall-Package NuGet.CommandLine -RemoveDependencies

		Write-Host "Installing 'NuGet.Analyze' into $env:LOCALAPPDATA\NuGet\Commands ..."

		$cmd = "$nugetToolsPath\nuget.exe"
		$args = "install -excludeVersion -outputDir '$env:LOCALAPPDATA\NuGet\Commands' NuGet.Analyze -Source 'https://go.microsoft.com/fwlink/?LinkID=206669'"

		invoke-expression "$cmd $args" | Out-Null
		Write-Host "Successfully installed 'NuGet.Analyze'."

		$analyzeCommandPackage = @(Get-Package NuGet.Analyze.Installer)[0]
		if(!$analyzeCommandPackage) {
            return $false
        }

		# Get the package path
		$analyzeCommandPath = Get-InstallPath $analyzeCommandPackage

		trap [System.Exception] {
			Remove-Item $nugetToolsPath -Recurse -Force -ErrorAction Continue
		}
		trap [System.Exception] {
			Remove-Item $analyzeCommandPath -Recurse -Force -ErrorAction Continue
		}
    #}

    return $true
}

ConfigureNuGetCommandLine