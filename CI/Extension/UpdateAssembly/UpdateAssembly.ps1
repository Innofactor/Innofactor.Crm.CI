[CmdletBinding()]

# Get variables from task
$AssemblyFile = Get-VstsInput -Name assembly -Require
$ConnectionString = Get-VstsInput -Name crmConnectionString -Require
$UpdateManaged = Get-VstsInput -Name updateManaged -AsBool
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path      : $scriptPath"
Write-Verbose "Assembly File    : $AssemblyFile"
Write-Verbose "Connection String: $ConnectionString"
Write-Verbose "Update Managed   : $UpdateManaged"

#Load DevUtils.CI
$DevUtilsCI = $scriptPath + "\ps_modules\DevUtils.CI\Cinteros.Crm.Utils.CI.dll"
Write-Verbose "Importing DevUtils.CI: $DevUtilsCI" 
Import-Module $DevUtilsCI
Write-Verbose "Imported DevUtils.CI"

Update-CrmAssembly -DLL $AssemblyFile -ConnectionString $ConnectionString -UM $UpdateManaged

