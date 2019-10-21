##-----------------------------------------------------------------------
## <copyright file="ApplyVersionToAssemblies.ps1">(c) Microsoft Corporation. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
##-----------------------------------------------------------------------
# Look for a 0.0.0.0 pattern in the build number. 
# If found use it to version the assemblies.
#
# For example, if the 'Build number format' build process parameter 
# $(BuildDefinitionName)_$(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)
# then your build numbers come out like this:
# "Build HelloWorld_2013.07.19.1"
# This script would then apply version 2013.07.19.1 to your assemblies.

# Enable -Verbose option
[CmdletBinding()]
param()

# If this script is not running on a build server, remind user to 
# set environment variables so that this script can be debugged
if(-not ($Env:BUILD_SOURCESDIRECTORY -and $Env:BUILD_BUILDNUMBER))
{
    Write-Error "You must set the following environment variables"
    Write-Error "to test this script interactively."
    Write-Host '$Env:BUILD_SOURCESDIRECTORY - For example, enter something like:'
    Write-Host '$Env:BUILD_SOURCESDIRECTORY = "C:\code\FabrikamTFVC\HelloWorld"'
    Write-Host '$Env:BUILD_BUILDNUMBER - For example, enter something like:'
    Write-Host '$Env:BUILD_BUILDNUMBER = "Build HelloWorld_0000.00.00.0"'
    exit 1
}

# Make sure path to source code directory is available
if (-not $Env:BUILD_SOURCESDIRECTORY)
{
    Write-Error ("BUILD_SOURCESDIRECTORY environment variable is missing.")
    exit 1
}
elseif (-not (Test-Path $Env:BUILD_SOURCESDIRECTORY))
{
    Write-Error "BUILD_SOURCESDIRECTORY does not exist: $Env:BUILD_SOURCESDIRECTORY"
    exit 1
}
Write-Verbose "BUILD_SOURCESDIRECTORY: $Env:BUILD_SOURCESDIRECTORY"

# Make sure there is a build number
if (-not $Env:BUILD_BUILDNUMBER)
{
    Write-Error ("BUILD_BUILDNUMBER environment variable is missing.")
    exit 1
}

Write-Host "BUILD_BUILDNUMBER: $Env:BUILD_BUILDNUMBER"

# Regular expression pattern to find the version in the build number 
# and then apply it to the assemblies
$VersionRegex = "\d+\.\d+\.\d+\.\d+"

# Getting input from UI
$VersionType	= Get-VstsInput -Name versionType -Require
$VersionFile	= Get-VstsInput -Name versionFile
$VersionMatch	= Get-VstsInput -Name versionMatch -AsBool

# File with resulting version
$VersionResult = Join-Path $Env:AGENT_BUILDDIRECTORY "version.txt"

$NewVersion = ""

# Checking if version should be taken from file and if that file does exist 
if ($VersionType -eq "file")
{
    if (Test-Path $VersionFile) 
    {
        $Base = Get-Content $VersionFile
        Write-Host "Base from file $VersionFile is $Base"
        
		if($localRevisionNumber)
		{
			$Revision = $localRevisionNumber;
	        Write-Host "Revision from local variable is $Revision"
	}
		else
		{
			$RevisionRegex = "\d+"
			$RevisionData = [regex]::matches($Env:BUILD_BUILDNUMBER,$RevisionRegex)
        
			$Revision = $RevisionData[$RevisionData.Count-1]
	        Write-Host "Revision from $Env:BUILD_BUILDNUMBER is $Revision"
        }

        $NewVersion = "$Base.$Revision"
    }
    else
    {
        Write-Error "File $VersionFile was not found, please check your build settings!"
        exit 1
    }
}
else
{
	# Get and validate the version data
    $VersionData = [regex]::matches($Env:BUILD_BUILDNUMBER,$VersionRegex)

    switch($VersionData.Count)
    {
       0        
          { 
             Write-Error "Could not find version number data in BUILD_BUILDNUMBER."
             exit 1
          }
       1 {}
       default 
          { 
             Write-Warning "Found more than instance of version data in BUILD_BUILDNUMBER." 
             Write-Warning "Will assume first instance is version."
          }
    }

    $NewVersion = $VersionData[0]
}

if ($VersionMatch)
{

}

Write-Host "Version: $NewVersion"

# Write version number to file, to be able to pick up in later scripts
# If working folder is the same, it can be read using:
# $version = Get-Content .\version.txt
Write-Host "Saving version in file $VersionResult"
Set-Content -Path $VersionResult -Value $NewVersion -Force

$PrefixVersions = Get-VstsInput -Name prefixVersions
$PrefixWord = Get-VstsInput -Name prefixWord
$PrefixBranch = Get-VstsInput -Name prefixBranch

if ($PrefixVersions -and ($Env:BUILD_SOURCEBRANCHNAME -eq $PrefixBranch))
{
    $SemVersion = $NewVersion -replace "(.*)\.(.*)", "`$1-$PrefixWord`$2"

    Write-Host "Pushing $SemVersion to environment variable"
    Write-Host "##vso[task.setvariable variable=SemanticVersion;]$SemVersion"
}

# Apply the version to the assembly property files
$files = gci $Env:BUILD_SOURCESDIRECTORY -recurse -include "*Properties*","My Project","*Version*" | 
    ?{ $_.PSIsContainer } | 
    foreach { gci -Path $_.FullName -Recurse -include *Info.cs }
if($files)
{
    Write-Host "Will apply $NewVersion to $($files.count) files."

    foreach ($file in $files) {
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -replace $VersionRegex, $NewVersion | Out-File $file
        Write-Host "$file - version applied"
    }
}
else
{
    Write-Warning "Warning: Found no files."
}
