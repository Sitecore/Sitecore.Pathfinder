restore-packages
================
Restores dependency packages as specified in the configuration

Remarks
-------
The `restore-packages` downloads dependency packages, that are specified in the `dependencies` configuration to the
[Project]/sitecore.project/packages directory.

Settings
--------
| Setting name   | Description                          | 
|----------------|--------------------------------------|
| dependencies:* | The dependency packages to download. |

Example
-------
```cmd
> scc restore-packages
```