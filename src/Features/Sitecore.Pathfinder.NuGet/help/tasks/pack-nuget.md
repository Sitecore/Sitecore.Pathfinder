pack-nuget
=================
Creates packages from the project.

Remarks
-------
The Nuget specifications and Nuget packages are located in the [Project]/sitecore.project folder.

Settings
--------
| Setting name               | Description                                      | 
|----------------------------|--------------------------------------------------|
| pack-nuget:base-path       | Base path relative to the project directory.     |
| pack-nuget:directory       | All .nuspec files in this directory are created. |
| pack-nuget:exclude         | Exclude glob.                                    |
| pack-nuget:include         | Include glob.                                    |
| pack-nuget:tokens          | Tokens in the .nuspec file that are replaced.    |

Example
-------
```cmd
> scc pack-nuget
```

