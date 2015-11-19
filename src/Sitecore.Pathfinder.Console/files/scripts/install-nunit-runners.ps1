Write-Output "Installing NUnit.Runners 2.6.4...";

$nuget = $toolsDirectory + "\nuget.exe";
$packagesDirectory = $projectDirectory + "\packages";
$scriptDirectory = $projectDirectory + "\sitecore.project\scripts";

& $nuget install NUnit.Runners -version 2.6.4 -o $packagesDirectory;

md $scriptDirectory | Out-Null;
$nunitConsole = $projectDirectory + "\packages\NUnit.Runners.2.6.4\tools\nunit-console.exe";

New-Item ($scriptDirectory + "\run-tests.ps1") -type file -force -value ("& $nunitConsole bin\MyAssembly.dll /nologo /nodots /trace=off");

Write-Output "";
Write-Output "*** REMEMBER TO CHANGE THE NAME OF THE TEST .DLL IN THE /sitecore.project/scripts/run-tests.ps1 FILE ***";