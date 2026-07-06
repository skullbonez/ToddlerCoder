@echo off
setlocal
pushd "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo .NET SDK was not found. Install the .NET 10 SDK or Visual Studio with the .NET desktop workload.
    pause
    exit /b 1
)

dotnet publish "%~dp0ToddlerCoder.csproj" -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
if errorlevel 1 (
    echo.
    echo Release publish failed.
    pause
    exit /b 1
)

echo.
echo Published exe:
echo %~dp0bin\Release\net10.0-windows\win-x64\publish\ToddlerCoder.exe
pause

popd
