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
$CI = $scriptPath + "\ps_modules\CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing CI module from: $CI" 
Import-Module $CI
Write-Verbose "CI module was successfully imported"

Update-CrmAssembly -DLL $AssemblyFile -ConnectionString $ConnectionString -UM $UpdateManaged