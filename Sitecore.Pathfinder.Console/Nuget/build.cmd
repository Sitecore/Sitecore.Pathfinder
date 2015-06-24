@echo off

if not exist "Nuget" goto build
cd Nuget 

:build
nuget pack SitecorePathfinder.nuspec -NoDefaultExcludes