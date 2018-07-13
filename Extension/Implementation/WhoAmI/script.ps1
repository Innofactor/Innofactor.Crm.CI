[CmdletBinding()]

# Get variables from task
$ConnectionString = Get-VstsInput -Name crmConnectionString -Require
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

Write-Verbose "Script Path      : $scriptPath"
Write-Verbose "Connection String: $ConnectionString"

#Load DevUtils.CI
$CI = $scriptPath + "\ps_modules\CI\Innofactor.Crm.CI.dll"
Write-Verbose "Importing CI module from: $CI" 
Import-Module $CI
Write-Verbose "CI module was successfully imported"

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