install-package
===========
Unpacks and installs the project package (including dependencies) in the website.

Remarks
-------
The package is installed by making a request to the [Website]/sitecore/shell/client/Applications/Pathfinder/InstallPackage with the name of the 
package on the query string. This webpage uses NuGet to unpack the files to the [DataFolder]/Pathfinder/Installed directory. Once the files 
are available, Pathfinder rebuilds the project and emits items and files to the website.

Any dependency packages are unpacked before the package and are processed in the same way.

Settings
--------
| Setting name                           | Description                                                       | 
|----------------------------------------|-------------------------------------------------------------------|
| install-package:check-bin-file-version | If true, check the versions of assemblies in /bin before copying. |
| install-package:install-url            | The URL for installing a package.                                 |

Example
-------
```cmd
> scc install-package
```

