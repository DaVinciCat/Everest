@echo off
setlocal

set "targetProject=src/Everest.Targets/Everest.Targets.csproj" 

if not exist "%outputDir%" (
    mkdir "%outputDir%"
)

echo Building...

dotnet build %targetProject% -p:TargetFramework=net6.0 --configuration Release
dotnet build %targetProject% -p:TargetFramework=net472 --configuration Release

if errorlevel 1 (
    echo Build failed for Everest.Targets Exiting...
    pause    
    exit /b 1
)  

echo.
echo Build completed.
endlocal
pause