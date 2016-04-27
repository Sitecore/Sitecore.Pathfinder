copy-dependencies
=================
Copies the dependency packages to the website.

Remarks
-------
The packages dependencies are Nuget packages. The packages are located in the sitecore.project/packages directory. 
To wrap a Sitecore package (.zip) in a Nuget package use the 'pack-dependencies' task.

Settings
--------
| Setting name                       | Description                                                      | 
|------------------------------------|------------------------------------------------------------------|
| copy-dependencies:source-directory | The directory that contains the packages to copy to the website. |

Example
-------
```cmd
> scc copy-dependencies
```

