write-exports
=============
Writes export declarations.

Remarks
-------
A project can declare items and resources that are used by other projects. The `write-exports` task writes export declarations of all
items and templates in the project to the [Project]/sitecore.project/exports.xml file.

When a project is being compiled, Pathfinder will look for NuGet packages in the [Project]/sitecore.project/packages directory and 
extract any exports.xml files. All declared items and templates are added to the project as external references.

Settings
--------
None.

Example
-------
```cmd
> scc write-exports
```