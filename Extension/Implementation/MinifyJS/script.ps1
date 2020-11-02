[CmdletBinding()]

$JsPath = Get-VstsInput -Name jsPath
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$gulpFile = Join-Path $scriptPath "gulpfile.js"

Write-Verbose "Script Path    : $scriptPath"
Write-Verbose "Gulp File      : $gulpFile"
Write-Verbose "JavaScript Path: $JsPath"

Write-Verbose "Copying $gulpFile to $JsPath"
Copy-Item $gulpFile $JsPath

Set-Location $JsPath

Write-Host "Installing Gulp"
npm install gulp gulp-rename gulp-strip-debug gulp-uglify gulp-terser

Write-Verbose "Calling gulp minify" 
node .\node_modules\gulp\bin\gulp.js minify

$gulpFile = Join-Path $JsPath "gulpfile.js"
 
Write-Verbose "Removing $gulpFile"
Remove-Item $gulpFile

Write-Verbose "Done"
