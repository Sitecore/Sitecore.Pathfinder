param($installPath, $toolsPath, $package, $project)
$fileInfo = new-object -typename System.IO.FileInfo -ArgumentList $project.FullName
$projectDirectory = $fileInfo.DirectoryName
robocopy "$toolsPath\.sitecore.tools" "$projectDirectory\.sitecore.tools" /mir /njh /njs /ndl /nc /ns /np
