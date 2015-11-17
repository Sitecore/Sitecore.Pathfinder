@echo off
rmdir /S /Q building\sitecore.project\extensions\unicorn
mkdir building\sitecore.project\extensions\unicorn
del Sitecore.Pathfinder.Unicorn.zip

xcopy /Y /S /Q bin\debug\*.* building\sitecore.project\extensions\unicorn\

powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('building', 'Unicorn.zip'); }"
mkdir ..\..\Sitecore.Pathfinder.Console\files\repository\sitecore.project\extensions\unicorn > nul
copy /Y Unicorn.zip ..\..\Sitecore.Pathfinder.Console\files\repository\sitecore.project\extensions\unicorn\