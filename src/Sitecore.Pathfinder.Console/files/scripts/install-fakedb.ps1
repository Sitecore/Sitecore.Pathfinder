Write-Output "Installing FakeDB 0.33.0...";

$nuget = $toolsDirectory + "\nuget.exe";
$packagesDirectory = $projectDirectory + "\packages";
$binDirectory = $projectDirectory + "\bin";

& $nuget install Sitecore.FakeDB -version 0.33.0 -o $packagesDirectory;

Write-Output "Coping FakeDb files...";
Copy-Item ($packagesDirectory + "\Sitecore.FakeDb.0.33.0\content\App.config") $projectDirectory;
Copy-Item ($packagesDirectory + "\Sitecore.FakeDb.0.33.0\content\App_Config") ($projectDirectory + "\App_Config") -Recurse;
Copy-Item ($packagesDirectory + "\Sitecore.FakeDb.0.33.0\lib\net45\Sitecore.FakeDb.dll") $binDirectory;

Write-Output "Coping license.xml from the website...";
Copy-Item ($dataFolderDirectory + "\license.xml") $projectDirectory;

Write-Output "Coping assemblies from the website to the /bin directory...";
Md $binDirectory | Out-Null;
Copy-Item ($websiteDirectory + "\bin\Lucene.Net.dll") $binDirectory;
Copy-Item ($websiteDirectory + "\bin\Sitecore.Analytics.dll") $binDirectory;
Copy-Item ($websiteDirectory + "\bin\Sitecore.Kernel.dll") $binDirectory;
Copy-Item ($websiteDirectory + "\bin\Sitecore.Logging.dll") $binDirectory;
Copy-Item ($websiteDirectory + "\bin\Sitecore.Nexus.dll") $binDirectory;
