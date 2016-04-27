run-script
==========
Runs a PowerShell, .cmd or .bat script.

Remarks
-------
It is possible to run scripts (PowerShell, .cmd or .bat) through Pathfinder, e.g. `scc install-fakedb.ps1`. Whenever a
task name ends with ".ps1", ".cmd" or ".bat", the task is assumed to be a script file.

Pathfinder will look in the [Project]/sitecore.project/scripts and [Tools]/files/scripts directories for the script file.

For PowerShell scripts Pathfinder passes the build context object, [Tools], [Project], [Website] and [Data] directories
as parameters.

Settings
--------
| Setting name   | Description                          | 
|----------------|--------------------------------------|
| dependencies:* | The dependency packages to download. |

Example
-------
```cmd
> scc install-helloworld.cmd
```