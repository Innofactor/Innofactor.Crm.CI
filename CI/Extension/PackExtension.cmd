@echo off

rem ===============================================================================
echo 
echo 
echo 
echo Assembling...

set dllpath=%~dp0..\bin\Release\*.dll
set extpath=%~dp0

echo Looking for: %dllpath%
echo Copying to : %extpath%

if EXIST "%extpath%ShuffleExport\ps_modules" (del "%extpath%ShuffleExport\ps_modules" /F /S /Q)
if EXIST "%extpath%ShuffleImport\ps_modules" (del "%extpath%ShuffleImport\ps_modules" /F /S /Q)
if EXIST "%extpath%UpdateAssembly\ps_modules" (del "%extpath%UpdateAssembly\ps_modules" /F /S /Q)
if EXIST "%extpath%UpdateWebResources\ps_modules" (del "%extpath%UpdateWebResources\ps_modules" /F /S /Q)
if EXIST "%extpath%WhoAmI\ps_modules" (del "%extpath%WhoAmI\ps_modules" /F /S /Q)

xcopy "%dllpath%" "%extpath%ShuffleExport\ps_modules\DevUtils.CI\"
xcopy "%dllpath%" "%extpath%ShuffleImport\ps_modules\DevUtils.CI\"
xcopy "%dllpath%" "%extpath%UpdateAssembly\ps_modules\DevUtils.CI\"
xcopy "%dllpath%" "%extpath%UpdateWebResources\ps_modules\DevUtils.CI\"
xcopy "%dllpath%" "%extpath%WhoAmI\ps_modules\DevUtils.CI\"
xcopy "%dllpath%.config" "%extpath%ShuffleExport\ps_modules\DevUtils.CI\"
xcopy "%dllpath%.config" "%extpath%ShuffleImport\ps_modules\DevUtils.CI\"
xcopy "%dllpath%.config" "%extpath%UpdateAssembly\ps_modules\DevUtils.CI\"
xcopy "%dllpath%.config" "%extpath%UpdateWebResources\ps_modules\DevUtils.CI\"
xcopy "%dllpath%.config" "%extpath%WhoAmI\ps_modules\DevUtils.CI\"

xcopy "%extpath%VstsTaskSdk" "%extpath%ApplyVersionToAssemblies\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%MinifyJS\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%ShuffleExport\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%ShuffleImport\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%UpdateAssembly\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%UpdateWebResources\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%WhoAmI\ps_modules\VstsTaskSdk\" /S /Y

rem ===============================================================================
echo 
echo 
echo 
echo Packing!

set /P updaterev=Update revision? [Y/N]: 
if [%updaterev%]==[y] (set updaterev=Y)
if [%updaterev%]==[Y] (set rev=--rev-version)

set /P publish=Publish? [Y/N]: 
if [%publish%]==[y] (set publish=Y)

if NOT [%publish%]==[Y] (
  goto tfx
)

set token=
if EXIST pat.txt (
  set /P token=<pat.txt
)
if [%token%]==[] (
  set /P token=Enter PAT: 
)

:tfx

if NOT EXIST VSIX (md VSIX)

if [%publish%]==[Y] (
  call tfx extension publish --manifest-globs vss-extension.json --output-path VSIX %rev% --token %token%
) else (
  call tfx extension create --manifest-globs vss-extension.json %rev% --output-path VSIX
)

rem Offer to save PAT in local file to be used next time

if EXIST pat.txt (
  goto end
)
if [%token%]==[] (
  goto end
)
echo.
set /P save=Save PAT to local file for use next time? [Y/N]: 
if [%save%]==[y] (
  set save=Y
)
if [%save%]==[Y] (
  echo %token%>pat.txt
  echo.
  echo Saved token to file pat.txt
  echo Be sure NOT to commit this file unintentionally!
  echo.
)

:end

pause
