[CmdletBinding()]

# Get variables from task
$PackageFile = Get-VstsInput -Name packageFile -Require
$ConnectionString = Get-VstsInput -Name crmConnectionString -Require
$UpdateManaged = Get-VstsInput -Name updateManaged -AsBool
$PackageName = Get-VstsInput -Name packageName -Require
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path      : $scriptPath"
Write-Verbose "Package File    : $PackageFile"
Write-Verbose "Package Name    : $PackageName"
Write-Verbose "Connection String: $ConnectionString"
Write-Verbose "Update Managed   : $UpdateManaged"

#Load DevUtils.CI
$CI = $scriptPath + "\ps_modules\CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing CI module from: $CI" 
Import-Module $CI
Write-Verbose "CI module was successfully imported"

Update-PluginPackage -PackageFile $PackageFile -PackageName $PackageName -ConnectionString $ConnectionString -UM $UpdateManaged