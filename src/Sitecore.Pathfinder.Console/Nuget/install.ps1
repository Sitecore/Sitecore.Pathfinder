param($installPath, $toolsPath, $package, $project)
$fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $project.FullName
$projectDirectory = $fileInfo.DirectoryName

Copy-Item "$toolsPath\sitecore.nuspec.rename" "$projectDirectory\sitecore.project\sitecore.nuspec"
Copy-Item "$toolsPath\.sitecore.tools\files\project\sitecore.project\dependencies" "$projectDirectory\sitecore.project\dependencies" -recurse

robocopy "$toolsPath\sitecore.tools" "$projectDirectory\sitecore.tools" /mir /njh /njs /ndl /nc /ns /np
