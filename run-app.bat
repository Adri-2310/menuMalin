@echo off
REM Script pour démarrer menuMalin en mode Hosted Blazor WebAssembly
REM Frontend + Backend sur le même port (7057)

echo.
echo ========================================
echo MenuMalin - Hosted Blazor WebAssembly
echo ========================================
echo.
echo Application: https://localhost:7057
echo Mode: Hosted (frontend + backend sur le même port)
echo.
echo Démarrage en cours...
echo.

cd /d "%~dp0"
dotnet run --project menuMalin.Server/menuMalin.Server.csproj --launch-profile https

echo.
echo Application fermée.
