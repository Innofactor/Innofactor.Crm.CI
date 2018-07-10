[CmdletBinding()]

# Get variables from task
$AssemblyFile = Get-VstsInput -Name assembly -Require
$KeyFile = Get-VstsInput -Name key
$Level = Get-VstsInput -Name key
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path       : $scriptPath"
Write-Verbose "Assembly File     : $AssemblyFile"
Write-Verbose "Key File          : $KeyFile"
Write-Verbose "Obfuscation Level : $Level"

#Load DevUtils.CI
$DevUtilsCI = $scriptPath + "\ps_modules\DevUtils.CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing DevUtils.CI: $DevUtilsCI" 
Import-Module $DevUtilsCI
Write-Verbose "Imported DevUtils.CI"

Out-ObfuscatedAssembly -DLL $AssemblyFile -Key $KeyFile -Obfuscation $Level