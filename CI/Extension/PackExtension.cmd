@echo off
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
