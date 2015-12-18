@echo off
rmdir /S /Q building\sitecore.project\extensions\markdown
mkdir building\sitecore.project\extensions\markdown
del Sitecore.Pathfinder.Markdown.zip

xcopy /Y /S /Q bin\debug\*.* building\sitecore.project\extensions\markdown\

powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('building', 'Markdown.zip'); }"
mkdir ..\..\Sitecore.Pathfinder.Console\files\repository\sitecore.project\extensions\markdown
copy /Y Markdown.zip ..\..\Sitecore.Pathfinder.Console\files\repository\sitecore.project\extensions\markdown\