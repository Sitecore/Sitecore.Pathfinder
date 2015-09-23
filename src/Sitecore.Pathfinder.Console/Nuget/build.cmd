@echo off

if not exist "Nuget" goto build
cd Nuget 

:build
copy ..\bin\Release\files\project\sitecore.project\sitecore.nuspec sitecore.nuspec.rename
nuget pack SitecorePathfinder.nuspec -NoDefaultExcludes