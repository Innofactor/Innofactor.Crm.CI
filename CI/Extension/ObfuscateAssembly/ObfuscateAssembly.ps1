[CmdletBinding()]

# Get variables from task
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path      : $scriptPath"

#Load DevUtils.CI
#$DevUtilsCI = $scriptPath + "\ps_modules\DevUtils.CI\Innofactor.Crm.CI.dll"
$DevUtilsCI = $scriptPath + "\..\..\bin\Debug\Innofactor.Crm.CI.dll"
Write-Verbose "Importing DevUtils.CI: $DevUtilsCI" 
Import-Module $DevUtilsCI
Write-Verbose "Imported DevUtils.CI"

Out-ObfuscatedAssembly -DLL "AssemblyFile" -Key "KeyFile" -Level 1  -Verbose
