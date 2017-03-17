@echo off

rem Add pre-build steps here

setlocal

dotnet %~dp0/Sitecore.Pathfinder.Console.dll %*

rem Add post-build steps here