[CmdletBinding()]

# Get variables from task
$ConnectionString = Get-VstsInput -Name crmConnectionString -Require
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path      : $scriptPath"
Write-Verbose "Connection String: $ConnectionString"

#Load DevUtils.CI
$DevUtilsCI = $scriptPath + "\ps_modules\DevUtils.CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing DevUtils.CI: $DevUtilsCI" 
Import-Module $DevUtilsCI
Write-Verbose "Imported DevUtils.CI"

$executingUser = Find-CrmUser -ConnectionString $ConnectionString -Verbose

if($executingUser)
{ 
Write-Host "Ping Succeeded userId: " $executingUser.UserId
Write-Host "BusinessUnitId: " $executingUser.BusinessUnitId
Write-Host "OrganizationId: " $executingUser.OrganizationId
}
else
{
Write-Host "Ping Failed"
}


