@echo off
setlocal

set "scc_exe=scc.exe"

if exist "%~dp0\sitecore.tools\scc.exe" (
    set "scc_exe=%~dp0\sitecore.tools\scc.exe"
)

if exist "%~dp0\node_modules\sitecore-pathfinder\scc.exe" (
    set "scc_exe=%~dp0\node_modules\sitecore-pathfinder\scc.exe"
)

"%scc_exe%" %*