install-package
===========
Unpacks and installs the project package (including dependencies) in the website.

Remarks
-------
The package is installed by making a request to the [Website]/sitecore/shell/client/Applications/Pathfinder/InstallPackage with the name of the 
package on the query string. This webpage uses NuGet to unpack the files to the [DataFolder]/Pathfinder/Installed directory. Once the files 
are available, Pathfinder rebuilds the project and emits items and files to the website.

Any dependency packages are unpacked before the package and are processed in the same way.

If you enable, 3 way merging, Pathfinder will not overwrite fields that have been changed in the database. Only fields that have
been changed in the project, and changed in the website, will be updated.

The is done through standard 3 way merging, where Pathfinder writes previous field values to a storage, and when installing
compares old and new values with the value in the storage.

The storage is located in [Data]/Pathfinder/.base/&lt;package&gt;/base.xml.

Settings
--------
| Setting name                           | Description                                                       | 
|----------------------------------------|-------------------------------------------------------------------|
| install-package:check-bin-file-version | If true, check the versions of assemblies in /bin before copying. |
| install-package:install-url            | The URL for installing a package.                                 |
| install-package:show-diagnostics       | If enabled, outputs project diagnostics. This is usually not needed as diagnostics are outputted during compilcation. |
| install-package:three-way-merge        | Enables 3 way merging for non-destructive installation.           |

Example
-------
```cmd
> scc install-package
```

