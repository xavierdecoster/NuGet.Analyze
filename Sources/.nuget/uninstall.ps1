function UninstallCommandLineExtensions {
	$targetPath = [System.IO.Path]::Combine($env:LOCALAPPDATA, "NuGet\Commands\NuGet.Analyze")
	DeleteFolderIfExists($targetPath)
}

function DeleteFolderIfExists {
	param ($targetPath)
	if ([System.IO.Directory]::Exists($targetPath)) {
		[System.IO.Directory]::Delete($targetPath)
	}
}

UninstallCommandLineExtensions