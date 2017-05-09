@echo off
echo Lib dir: %1
echo Prj dir: %2

set dllpath=%1
set dllpath=%dllpath:"=%*.dll

set extpath=%2
set extpath=%extpath:"=%Extension\

echo Looking for: %dllpath%
echo Copying to : %extpath%

if EXIST "%extpath%ShuffleExport\ps_modules" (del "%extpath%ShuffleExport\ps_modules" /F /S /Q)
if EXIST "%extpath%ShuffleImport\ps_modules" (del "%extpath%ShuffleImport\ps_modules" /F /S /Q)
if EXIST "%extpath%UpdateAssembly\ps_modules" (del "%extpath%UpdateAssembly\ps_modules" /F /S /Q)
if EXIST "%extpath%UpdateWebResources\ps_modules" (del "%extpath%UpdateWebResources\ps_modules" /F /S /Q)

xcopy "%dllpath%" "%extpath%ShuffleExport\ps_modules\DevUtils.CI\"
xcopy "%dllpath%" "%extpath%ShuffleImport\ps_modules\DevUtils.CI\"
xcopy "%dllpath%" "%extpath%UpdateAssembly\ps_modules\DevUtils.CI\"
xcopy "%dllpath%" "%extpath%UpdateWebResources\ps_modules\DevUtils.CI\"

xcopy "%extpath%VstsTaskSdk" "%extpath%ApplyVersionToAssemblies\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%MinifyJS\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%ShuffleExport\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%ShuffleImport\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%UpdateAssembly\ps_modules\VstsTaskSdk\" /S /Y
xcopy "%extpath%VstsTaskSdk" "%extpath%UpdateWebResources\ps_modules\VstsTaskSdk\" /S /Y
