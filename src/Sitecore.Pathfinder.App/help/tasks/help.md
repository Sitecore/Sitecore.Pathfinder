help
====
Displays version information and a list of commands.

Remarks
-------
None.

Settings
---------
None

Example
-------
```cmd
> scc help

Welcome to Sitecore Pathfinder.

To create a new Sitecore Pathfinder project, run 'scc new-project' command in an empty directory.

Version: 0.7.0.13896

SYNTAX: scc [task name] [options]

EXAMPLES: scc
          scc new-project
          scc check-project

REMARKS:
To get additional help for a task, use:
    scc help [task]

TASKS:
add-project - Adds a Pathfinder project to an existing directory.
check-project - Checks the project for warnings and errors.
clean-website - Removes Pathfinder files and assemblies from the website.
copy-dependencies - Copies the dependency packages to the website.
copy-package - Copies the project output to the website.
find-references - Finds all project items that the specified project item references.
find-usages - Finds all project items that references the specified project item.
generate-code - Generates code from items and files in the project.
help - Displays version information and a list of commands.
import-website - Imports items and files from the website.
init-atom - Creates a new Atom project.
init-visualstudio - Creates a new Visual Studio project.
init-vscode - Creates a new Visual Studio Code project.
install-package - Unpacks and installs the project package (including dependencies) in the website.
install-project - Installs the project directly from the project directory.
list-files - Lists the files in the project.
list-items - Lists the Sitecore items in the project.
list-project - Lists the project items (Sitecore items and files).
list-rules - Lists the available conditions and actions.
new-project - Creates a new Pathfinder project.
pack-dependencies - Creates a Nuget package for Sitecore package in the sitecore.tools\packages directory.
pack-npm - Creates an npm module from the project.
pack-nuget - Creates packages from the project.
publish-database - Publishes a Sitecore database (usually the master database).
reset-website - Resets the website.
restore-packages - Restores dependency packages as specified in the configuration
run-script - Runs a PowerShell, .cmd or .bat script.
show-config - Displays the effective configuration

show-metrics - Shows various information about the project.
show-website - Open the default webpage in a browser, if a start URL has been defined

sync-website - Synchronizes project and the website.
troubleshoot-website - Tries to fix a non-working website.
update-mappings - Updates the project/website mapping on the website.
watch-project - Watches the project directory and install changes immediately.
write-exports - Writes export declarations
write-serialization - Writes all items to a serialization folder
write-website-exports - Write website exports.

SCRIPTS:
install-cleanblog.cmd
install-fakedb.ps1
install-grunt.cmd
install-helloworld.cmd
install-nunit-runners.ps1
install-todomvc.cmd
install-vs.cmd
```

