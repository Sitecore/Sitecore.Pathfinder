@echo off
del ..\Sitecore.Pathfinder.zip
rmdir /Q /S bin\pack
mkdir bin\pack
xcopy bin\release\files bin\pack\sitecore.tools\files\* /s 
xcopy bin\release\*.dll bin\pack\sitecore.tools\* 
xcopy bin\release\scc.exe bin\pack\sitecore.tools
xcopy bin\release\scc.exe.config bin\pack\sitecore.tools
xcopy bin\release\scconfig.json bin\pack\sitecore.tools

powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('bin\pack', '..\Sitecore.Pathfinder.zip'); }"

rmdir /Q /S bin\pack

call Nuget\build.cmd