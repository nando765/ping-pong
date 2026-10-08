@echo off
setlocal enabledelayedexpansion

set UNITY=C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe
set PROJECT=%~dp0
set OUT=%PROJECT%ejecutable
set LOG=%OUT%\build.log
set ERRLOG=%OUT%\editor_errors.log
set CSPROJ=%PROJECT%Assets\Editor\CommandLineBuild.cs

:: Recrear carpeta de salida limpia
if exist "%OUT%" rmdir /s /q "%OUT%"
mkdir "%OUT%"

echo ========================================
echo  Unity Batch Build - Ping-Pong
echo  Project: %PROJECT%
echo  Output:  %OUT%
echo  Unity:   %UNITY%
echo ========================================
echo.

"%UNITY%" -batchmode -quit -projectPath "%PROJECT%" -buildTarget StandaloneWindows64 -executeMethod CommandLineBuild.Build -logFile "%LOG%" -nographics -umask 022

set EXITCODE=%ERRORLEVEL%

echo.
echo ========================================
echo  BUILD FINISHED - EXIT CODE: %EXITCODE%
echo ========================================
echo.

if %EXITCODE% neq 0 (
    echo [ERROR] La construccion fallo. Revisar:
    echo   %LOG%
    echo   %ERRLOG%
    goto :end
)

if not exist "%OUT%\ping-pong.exe" (
    echo [ERROR] No se creo ping-pong.exe
    goto :end
)

echo [OK] Ejecutable creado:
echo   %OUT%\ping-pong.exe
echo.
echo Contenido del paquete:
dir /s /b "%OUT%" | findstr /v "build.log" | findstr /v "editor_errors.log"
echo.
echo Tamaño total de la carpeta ejecutable:
for /f "tokens=3" %%i in ('dir /s /-c "%OUT%" ^| findstr /r "^[ ]*[0-9]+$"') do set SIZE=%%i
echo   %SIZE% bytes
echo.
echo Checksum SHA1 del ejecutable:
certutil -hashfile "%OUT%\ping-pong.exe" SHA1 | findstr /v "SHA1" | findstr /v " hash" | findstr /v "CertUtil"
echo.
echo Para ejecutar el juego usar:
echo   "%OUT%\ping-pong.exe"
echo.
echo La primera escena cargada es: Menu (escena del menu)
echo Luego se puede acceder a SampleScene desde el boton "Jugar"
echo.

:end
endlocal
exit /b %EXITCODE%
PAUSE
