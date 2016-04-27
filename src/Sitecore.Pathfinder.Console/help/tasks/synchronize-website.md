sync-website
===================
Synchronizes project and the website.

Remarks
-------
In Pathfinder the project contains the whole truth. However a project may need to use items, template, renderings from a standard 
Sitecore website. If these resources are not available as packages, you can generate the using Pathfinder.

These external resources can be imported into the project by using the `sync-website` task. This task makes a request to the website
to collect the needed information. The information is downloaded as a zip file and unpacked in the [Project] directory.

The sync-website task is configured on the 'sync-website' section in the scconfig.json configuration file.

Settings
--------
| Setting name   | Description             | 
|----------------|-------------------------|
| sync-website:* | The data to synchronize |

Example
-------
```cmd
> scc sync-website
```