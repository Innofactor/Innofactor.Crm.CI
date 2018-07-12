[CmdletBinding()]

# Get variables from task
$PatternFile = Get-VstsInput -Name pattern -Require
$Prefix = Get-VstsInput -Name prefix -Require
$RootPath = Get-VstsInput -Name rootPath -Require
$ConnectionString = Get-VstsInput -Name crmConnectionString -Require
$UpdateManaged = Get-VstsInput -Name updateManaged -AsBool
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path      : $scriptPath"
Write-Verbose "Root Path        : $RootPath"
Write-Verbose "Prefix           : $Prefix"
Write-Verbose "Pattern File     : $PatternFile"
Write-Verbose "Connection String: $ConnectionString"
Write-Verbose "Update Managed   : $UpdateManaged"

#Load DevUtils.CI
$DevUtilsCI = $scriptPath + "\ps_modules\DevUtils.CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing DevUtils.CI: $DevUtilsCI" 
Import-Module $DevUtilsCI
Write-Verbose "Imported DevUtils.CI"

Update-CrmResources -R $RootPath -PF $PatternFile -Pre $Prefix -ConnectionString $ConnectionString -UM $UpdateManaged