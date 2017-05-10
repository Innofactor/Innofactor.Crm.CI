@echo off
echo Packing!

set /P updaterev=Update revision? [Y/N]: 
if [%updaterev%]==[y] (set updaterev=Y)
if [%updaterev%]==[Y] (set rev=--rev-version)

set /P publish=Publish? [Y/N]: 
if [%publish%]==[y] (set publish=Y)

if [%publish%]==[Y] (
  set /P pat=Enter PAT: 
)
if NOT EXIST VSIX (md VSIX)

if [%publish%]==[Y] (
  call tfx extension publish --manifest-globs vss-extension.json --output-path VSIX %rev% --token %pat%
) else (
  call tfx extension create --manifest-globs vss-extension.json %rev% --output-path VSIX
)

pause

rem   set /P share=Enter shares: 
rem   call tfx extension publish --manifest-globs vss-extension.json --output-path VSIX %rev% --share-with %share% --token %pat%
