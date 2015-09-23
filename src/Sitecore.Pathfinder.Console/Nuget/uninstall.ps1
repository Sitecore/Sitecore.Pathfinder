param($installPath, $toolsPath, $package, $project)
$fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $project.FullName
$projectDirectory = $fileInfo.DirectoryName
Remove-Item "$projectDirectory\.sitecore.tools" -Force -Recurse