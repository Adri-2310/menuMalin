@echo off
REM Script pour démarrer le serveur backend et frontend
REM Backend: https://localhost:7057
REM Frontend: https://localhost:7777

echo.
echo ========================================
echo MenuMalin Development Servers Startup
echo ========================================
echo.
echo Backend (Server):  https://localhost:7057
echo Frontend (Client): https://localhost:7777
echo.
echo Démarrage en cours...
echo.

REM Créer deux fenêtres de terminal
start "MenuMalin Backend Server" cmd /k "cd menuMalin.Server && dotnet run"
timeout /t 3

start "MenuMalin Frontend Client" cmd /k "cd menuMalin && dotnet run"

echo.
echo ✓ Les deux serveurs se lancent dans des fenêtres séparées
echo ✓ Attendez que les serveurs soient prêts (environ 10-15 secondes)
echo ✓ Frontend: https://localhost:7777
echo ✓ Ctrl+C pour arrêter chaque serveur
echo.
