@echo off
setlocal

set "outputDir=binaries" 
set "targetProject=src/Everest.Targets/Everest.Targets.csproj" 

if not exist "%outputDir%" (
    mkdir "%outputDir%"
)

echo Building...
    dotnet build %targetProject% -p:TargetFramework=net6.0 --configuration Release -p:Platform=x64 -p:DebugType=None --output "%outputDir%"

    if errorlevel 1 (
        echo Build failed for Everest.Targets Exiting...
        pause    
        exit /b 1
    )  

del %outputDir%\*.deps.json
del %outputDir%\*Everest.Targets.dll

echo.
echo Build completed.
endlocal
pause