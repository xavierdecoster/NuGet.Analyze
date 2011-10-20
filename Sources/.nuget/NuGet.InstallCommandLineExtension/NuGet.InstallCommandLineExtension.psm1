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

function Install-CommandLineExtension {
	param (
		[Parameter(Mandatory=$True)]
		[System.String]
		[Alias("Package")]
		$packageId,

		[Parameter(Mandatory=$False)]
		[System.String]
		[Alias("Source")]
		$packageSource = 'https://go.microsoft.com/fwlink/?LinkID=206669'
	)

	# Install the nuget command line in a temp location
	$tmpDir = [System.Guid]::NewGuid().ToString()
    $nugetToolsPath = (Join-Path $env:TEMP $tmpDir)
    Install-Package NuGet.CommandLine -Source $packageSource

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

	Write-Host "Installing '$packageId' into $env:LOCALAPPDATA\NuGet\Commands ..."

	$cmd = "$nugetToolsPath\nuget.exe"
	$outputDir = "$env:LOCALAPPDATA\NuGet\Commands"
	$expression = [System.String]::Format("& {0} install -excludeVersion -outputDir '{1}' {2} -Source '{3}'", $cmd, $outputDir, $packageId, $packageSource)

	Write-Host $expression

	Invoke-Expression $expression | Out-Null
	Write-Host "Successfully installed '$packageId'."

	$analyzeCommandPackage = @(Get-Package NuGet.Analyze.Installer)[0]
	if(!$analyzeCommandPackage) {
        return $false
    }

    return $true
}

Export-ModuleMember Install-CommandLineExtension