@echo off
setlocal

echo Packing nupkg...

nuget pack everest.nuspec

if errorlevel 1 (
    echo Nuget pack failed for Everest.Targets Exiting...
    pause    
    exit /b 1
) 

echo.
echo Pack completed.
endlocal
pause