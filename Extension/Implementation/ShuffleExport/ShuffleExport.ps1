[CmdletBinding()]

# Get variables from task
$DefinitionFile = Get-VstsInput -Name definitionFile -Require
$DataFile = Get-VstsInput -Name dataFile
$SetVersion = Get-VstsInput -Name setVersion -AsBool
$ConnectionString = Get-VstsInput -Name crmConnectionString -Require

if (!$DataFile -or ($DataFile = $env:BUILD_SOURCESDIRECTORY)) {
	Write-Verbose "Setting default data file"
	$DataFile = [io.path]::ChangeExtension($DefinitionFile, ".data.xml")
}

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$definitionPath = Split-Path -Parent $DefinitionFile

Write-Verbose "Script Path    : $scriptPath"
Write-Verbose "Definition Path: $definitionPath"
Write-Verbose "Definition File: $DefinitionFile"
Write-Verbose "Data File      : $DataFile"
Write-Verbose "Set Version    : $SetVersion"
Write-Verbose "Connection String: $ConnectionString"

if ($SetVersion -and (Get-Content $DefinitionFile | Select-String "{ShuffleVar:version}" -Quiet)) {
	Write-Verbose "Definition contains version placeholder"
	$versionFile = "version.txt"
	if ($env:AGENT_BUILDDIRECTORY) {
		$versionFile = Join-Path $env:AGENT_BUILDDIRECTORY $versionFile
	}
	Write-Verbose "Version File: $versionFile"
	$version = Get-Content $versionFile
	Write-Verbose "New version is $version"
	$definitionFixed = [io.path]::ChangeExtension($DefinitionFile, ".fixed.xml")
	(Get-Content $DefinitionFile).replace('{ShuffleVar:version}', $version) | Set-Content $definitionFixed
	Write-Host "Loading fixed definition from $definitionFixed"
	[xml]$Definition = Get-Content $definitionFixed
}
else {
	Write-Host "Loading definition from $DefinitionFile"
	[xml]$Definition = Get-Content $DefinitionFile
}

#Load DevUtils.CI
$DevUtilsCI = $scriptPath + "\ps_modules\DevUtils.CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing DevUtils.CI: $DevUtilsCI" 
Import-Module $DevUtilsCI
Write-Verbose "Imported DevUtils.CI"

Write-Host "Starting export"

[xml]$exp = Export-CrmShuffle -ConnectionString $ConnectionString -Definition $Definition -Folder $definitionPath -Type "SimpleWithValue"

if ($definitionFixed) {
	Write-Verbose "Removing fixed definition file"
	Remove-Item $definitionFixed
}

if (!$exp) {
	Write-Host "No data file result"
}
else {
	Write-Host "Writing export result to $DataFile"
	[xml]$exp.Save($DataFile)
}

