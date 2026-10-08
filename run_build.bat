@echo off
set UNITY="C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe"
set PROJECT="C:\Users\ALEJANDRO\Desktop\Nueva carpeta (19)\unit\ping-pong"
set OUT=%PROJECT%\ejecutable
set LOG=%OUT%\build.log
set ERR=%OUT%\editor_errors.log
%UNITY% -batchmode -quit -projectPath %PROJECT% -buildTarget StandaloneWindows64 -executeMethod CommandLineBuild.Build -logFile %LOG% -nographics -umask 022
echo BUILD EXIT CODE: %ERRORLEVEL%
type %LOG%
type %ERR%
