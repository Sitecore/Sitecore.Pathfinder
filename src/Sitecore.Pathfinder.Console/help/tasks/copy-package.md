copy-package
=================
Copies the project output to the website.

Remarks
-------
The `copy-package` task copies any NuGet packages, that are built, to the website.

Settings
--------
| Setting name                   | Description                                             | 
|--------------------------------|---------------------------------------------------------|
| copy-package:package-directory | The directory under the website Data Folder to copy to. |

Example
-------
```cmd
> scc copy-package
```

