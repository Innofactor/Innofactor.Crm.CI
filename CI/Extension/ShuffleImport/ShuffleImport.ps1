[CmdletBinding()]

# Get variables from task
$DefinitionFile = Get-VstsInput -Name definitionFile -Require
$DataFile = Get-VstsInput -Name dataFile
$ConnectionString = Get-VstsInput -Name crmConnectionString -Require

if ([string]::IsNullOrEmpty($DataFile) -or ($DataFile -eq $env:BUILD_SOURCESDIRECTORY) -or ($DataFile -eq $env:SYSTEM_ARTIFACTSDIRECTORY)) {
	Write-Verbose "Setting default data file"
	$DataFile = [io.path]::ChangeExtension($DefinitionFile, ".data.xml")
}

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$definitionPath = Split-Path -Parent $DefinitionFile

Write-Verbose "Script Path      : $scriptPath"
Write-Verbose "Definition Path  : $definitionPath"
Write-Verbose "Definition File  : $DefinitionFile"
Write-Verbose "Data File        : $DataFile"
Write-Verbose "Connection String: $ConnectionString"

Write-Host "Loading definition from $DefinitionFile"
[xml]$Definition = Get-Content $DefinitionFile

#Load DevUtils.CI
$DevUtilsCI = $scriptPath + "\ps_modules\DevUtils.CI\Cinteros.Crm.Utils.CI.dll"
Write-Verbose "Importing DevUtils.CI: $DevUtilsCI" 
Import-Module $DevUtilsCI
Write-Verbose "Imported DevUtils.CI"

if (Test-Path $DataFile) {
	Write-Host "Loading data file"
	[xml]$Data = Get-Content $DataFile
}
else {
	Write-Verbose "Data file does not exist"
	$Data = $null
}

Write-Host "Starting import"

$imp = Import-CrmShuffle -ConnectionString $ConnectionString -Definition $Definition -DataXml $Data -Folder $definitionPath

Write-Host "Created: " $imp.Created
Write-Host "Updated: " $imp.Updated
Write-Host "Skipped: " $imp.Skipped
Write-Host "Deleted: " $imp.Deleted
Write-Host "Failed : " $imp.Failed


