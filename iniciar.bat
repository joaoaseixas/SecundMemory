@echo off
cd /d "%~dp0"
start "" http://localhost:5005
powershell -NoProfile -ExecutionPolicy Bypass -File run.ps1 %*
