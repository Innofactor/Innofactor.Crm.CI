[CmdletBinding()]

# Get variables from task
$AssemblyFile = Get-VstsInput -Name assembly -Require
$KeyFile = Get-VstsInput -Name key
$Level = Get-VstsInput -Name level
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path       : $scriptPath"
Write-Verbose "Assembly File     : $AssemblyFile"
Write-Verbose "Key File          : $KeyFile"
Write-Verbose "Obfuscation Level : $Level"

#Load DevUtils.CI
$CI = $scriptPath + "\ps_modules\CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing CI module from: $CI" 
Import-Module $CI
Write-Verbose "CI module was successfully imported"

Out-ObfuscatedAssembly -DLL $AssemblyFile -Key $KeyFile -Obfuscation $Level