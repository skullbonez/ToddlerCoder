@echo off
setlocal
pushd "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
    echo .NET SDK was not found. Install the .NET 10 SDK or Visual Studio with the .NET desktop workload.
    pause
    exit /b 1
)

dotnet run --project "%~dp0ToddlerCoder.csproj" -- --debug-windowed
if errorlevel 1 (
    echo.
    echo ToddlerCoder did not start successfully.
    pause
    exit /b 1
)

popd
