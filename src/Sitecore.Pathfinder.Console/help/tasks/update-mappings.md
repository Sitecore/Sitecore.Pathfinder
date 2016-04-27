update-mappings
===================
Updates the project/website mapping on the website.

Remarks
-------
The 'project-website-mappings' settings enables the serializing data provider on the website to serialize changed items back to the projects.
This task should be called when the 'project-website-mappings' settings have been changed (or you can just kill the 'w3wp.exe' process or restart IIS).

Settings
--------
None.

Example
-------
```cmd
> scc update-mappings
```