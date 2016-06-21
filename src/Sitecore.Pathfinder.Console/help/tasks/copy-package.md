copy-package
=================
Copies the project output to the website.

Remarks
-------
The `copy-package` task copies any output packages (NuGet or Npm), that are built, to the specified directories.

This is useful when having multiple project the references each other.

Settings
--------
| Setting name                   | Description                                             | 
|--------------------------------|---------------------------------------------------------|
| copy-package:*                 | List of file patterns and target directories.           |

Example
-------
```cmd
> scc copy-package
```

